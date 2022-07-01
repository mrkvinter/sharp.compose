using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SharpCompose.Base;

internal static class ComposeKey
{
    private static readonly Dictionary<MethodBase, int> MethodIds = new();
    private static readonly Stack<LoopIndexController> LoopIndexControllers = new();
    private static int idCounter;

    private static string LoopIndex => string.Join('-', LoopIndexControllers);

    public sealed class LoopIndexController : IDisposable
    {
        private int index;

        internal LoopIndexController()
        {
            LoopIndexControllers.Push(this);
        }

        public void Next(int newIndex) => index = newIndex;

        public void Dispose()
        {
            LoopIndexControllers.Pop();
        }

        public override string ToString() => index.ToString();
    }

    public static LoopIndexController StartLoopIndex() => new();


    [ComposableApi]
    public static string GetKey(string postfix = "")
    {
        var st = new StackTrace();
        var key = new StringBuilder();
        var stackFrames = st.GetFrames();

        foreach (var stackFrame in stackFrames)
        {
            var methodBase = stackFrame.GetMethod();
            if (methodBase?.GetCustomAttribute<ComposableApiAttribute>() is { } apiAttribute)
            {
                if (apiAttribute is RootComposableApiAttribute)
                    break;

                continue;
            }

            if (!MethodIds.TryGetValue(methodBase, out var id))
            {
                id = idCounter++;
                MethodIds.Add(methodBase, id);
            }
            key.Append($"{stackFrame.GetNativeOffset()}({id})-");
        }

        key.Append(LoopIndex);
        key.Append(postfix);

        return key.ToString();
    }
}