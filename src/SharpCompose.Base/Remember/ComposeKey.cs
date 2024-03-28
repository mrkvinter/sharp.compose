using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SharpCompose.Base;

internal static class ComposeKey
{
    private static readonly Dictionary<MethodBase, int> MethodIds = new();
    private static int idCounter;

    [ComposableApi]
    public static string GetKey(string postfix = "")
    {
        var st = new StackTrace();
        var key = new StringBuilder();
        var stackFrames = st.GetFrames();
        var group = Composer.Instance.CurrentGroup;
        var groupRootVisited = false;
        key.Append(group.Id);

        foreach (var stackFrame in stackFrames)
        {
            var methodBase = stackFrame.GetMethod()!;
            if (methodBase.GetCustomAttribute<ComposableApiAttribute>() is { } apiAttribute)
            {
                if (apiAttribute is RootComposableApiAttribute)
                    break;

                if (apiAttribute is GroupRootComposableApiAttribute)
                {
                    if (!Composer.Instance.WaitingInsertGroup || groupRootVisited)
                        break;
                    
                    groupRootVisited = true;
                }

                continue;
            }

            if (!MethodIds.TryGetValue(methodBase, out var id))
            {
                id = idCounter++;
                MethodIds.Add(methodBase, id);
            }

            key.Append($"-{stackFrame.GetNativeOffset()}({id})");
        }

        key.Append(postfix);

        var keyString = key.ToString();
        if (group.CountNodes.TryGetValue(keyString, out var count))
        {
            count++;
            group.CountNodes[keyString] = count;
        }
        else
        {
            group.CountNodes.Add(keyString, 0);
        }

        key.Append(count);

        return key.ToString();
    }
}