using System.Collections.Generic;

namespace TestSharpCompose.TestComposer;

public static class HtmlNodeExtensions
{
    public static HtmlNode? GetById(this HtmlNode node, string id)
    {
        var stack = new Stack<HtmlNode>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            if (currentNode.Id == id)
                return currentNode;
            
            foreach (var htmlNode in currentNode.Child)
            {
                stack.Push(htmlNode);
            }
        }

        return null;
    }
}