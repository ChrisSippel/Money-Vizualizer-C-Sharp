using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer.Helpers.EventHandlers
{
    // Borowed from: http://paulstovell.com/blog/weakevents
    [DebuggerNonUserCode]
    public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            _method = callback.Method;
            _targetReference = new WeakReference(callback.Target, true);
        }

        [DebuggerNonUserCode]
        public void Handler(object sender, TEventArgs e)
        {
            var target = _targetReference.Target;
            if (target == null)
            {
                return;
            }

            var callback = (Action<object, TEventArgs>)Delegate.CreateDelegate(typeof(Action<object, TEventArgs>), target, _method, true);
            callback?.Invoke(sender, e);
        }
    }
}
