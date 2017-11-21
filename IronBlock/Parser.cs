using System;
using System.Collections.Generic;
using System.Xml;

namespace IronBlock
{
    public class Parser
    {
        IDictionary<string,Type> blocks = new Dictionary<string,Type>();

        public void AddBlock<T>(string type) where T : IBlock
        {
            if (this.blocks.ContainsKey(type))
            {
                this.blocks[type] = typeof(T);
                return;
            }
            this.blocks.Add(type, typeof(T));
        }

        public Workspace Parse(string xml)
        {
            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            var workspace = new Workspace();
            foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
            {
                if (node.LocalName == "block" )
                {
                    var block = ParseBlock(node);
                    workspace.Blocks.Add(block);
                }
            }

            return workspace;
        }

        IBlock ParseBlock(XmlNode node)
        {
            var type = node.Attributes["type"].Value;
            if (!this.blocks.ContainsKey(type)) throw new ApplicationException($"block type not registered: '{type}'");
            var block = Activator.CreateInstance(this.blocks[type]) as IBlock;
            
            block.Type = type;
            block.Id = node.Attributes["id"]?.Value;

            foreach (XmlNode childNode in node.SelectNodes("mutation")) ParseMutation(childNode, block);
            foreach (XmlNode childNode in node.SelectNodes("field")) ParseField(childNode, block);
            foreach (XmlNode childNode in node.SelectNodes("value")) ParseValue(childNode, block);
            foreach (XmlNode childNode in node.SelectNodes("statement")) ParseStatement(childNode, block);
            foreach (XmlNode childNode in node.SelectNodes("next"))
            {
                block.Next = ParseBlock(childNode.FirstChild);
            }

            return block;
        }

        void ParseField(XmlNode fieldNode, IBlock block)
        {
            var field = new Field
            {
                Name = fieldNode.Attributes["name"]?.Value,
                Value = fieldNode.InnerText
            };
            block.Fields.Add(field);
        }

        void ParseValue(XmlNode valueNode, IBlock block)
        {
            var childNode = valueNode.SelectSingleNode("block");
            var childBlock = ParseBlock(childNode);

            var value = new Value
            {
                Name = valueNode.Attributes["name"]?.Value,
                Block = childBlock
            };
            block.Values.Add(value);
        }


        void ParseStatement(XmlNode statementNode, IBlock block)
        {
            var childNode = statementNode.SelectSingleNode("block");
            var childBlock = ParseBlock(childNode);

            var statement = new Statement
            {
                Name = statementNode.Attributes["name"]?.Value,
                Block = childBlock
            };
            block.Statements.Add(statement);
        }

        void ParseMutation(XmlNode mutationNode, IBlock block)
        {
            foreach (XmlAttribute attribute in mutationNode.Attributes)
            {
                if ( block.Mutations.ContainsKey(attribute.Name))
                {
                    block.Mutations[attribute.Name] = attribute.Value;
                }
                else
                {
                    block.Mutations.Add(attribute.Name, attribute.Value);
                }
            }
        }

    }


}