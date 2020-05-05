using Prism.Interactivity.InteractionRequest;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ModuleA.Extensions {
    public static class InteractionRequestExtension {
        public static IObservable<T> RaiseAsObservable<T>(this InteractionRequest<T> self, T notification) where T : INotification {
            var s = new AsyncSubject<T>();
            self.Raise(notification, x => { s.OnNext(x); s.OnCompleted(); });
            return s.AsObservable();
        }
    }
}
