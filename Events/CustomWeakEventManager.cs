namespace Com.MarcusTS.SharedUtils.Events
{
   using Com.MarcusTS.SharedUtils.Utils;
   using System;
   using System.Collections.Generic;
   using System.Reflection;
   using System.Runtime.CompilerServices;

   /// <summary>
   ///    Copied from (internal) Xamarin.Forms  to make it publicly accessible.
   /// </summary>
   public static class CustomWeakEventManager
   {
      /// <summary>
      ///    The event handlers
      /// </summary>
      private static readonly Dictionary<string, List<Subscription>> _eventHandlers =
         new Dictionary<string, List<Subscription>>();

      /// <summary>
      ///    Adds the event handler.
      /// </summary>
      /// <typeparam name="TEventArgs">The type of the t event arguments.</typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="ArgumentNullException">
      ///    eventName
      ///    or
      ///    handler
      /// </exception>
      public static void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler,
                                                     [CallerMemberName] string eventName = null)
         where TEventArgs : EventArgs
      {
         if (eventName.IsEmpty())
         {
            throw new ArgumentNullException(nameof(eventName));
         }

         if (handler == null)
         {
            throw new ArgumentNullException(nameof(handler));
         }

         AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      /// <summary>
      ///    Adds the event handler.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="ArgumentNullException">
      ///    eventName
      ///    or
      ///    handler
      /// </exception>
      public static void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
      {
         if (eventName.IsEmpty())
         {
            throw new ArgumentNullException(nameof(eventName));
         }

         if (handler == null)
         {
            throw new ArgumentNullException(nameof(handler));
         }

         AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      /// <summary>
      ///    Handles the event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The arguments.</param>
      /// <param name="eventName">Name of the event.</param>
      public static void HandleEvent(object sender, object args, string eventName)
      {
         var toRaise = new List<(object subscriber, MethodInfo handler)>();
         var toRemove = new List<Subscription>();

         if (_eventHandlers.TryGetValue(eventName, out var target))
         {
            for (var i = 0; i < target.Count; i++)
            {
               var subscription = target[i];
               var isStatic = subscription.Subscriber == null;
               if (isStatic)
               {
                  // For a static method, we'll just pass null as the first parameter of MethodInfo.Invoke
                  toRaise.Add((null, subscription.Handler));
                  continue;
               }

               var subscriber = subscription.Subscriber.Target;

               if (subscriber == null)

               // The subscriber was collected, so there's no need to keep this subscription around
               {
                  toRemove.Add(subscription);
               }
               else
               {
                  toRaise.Add((subscriber, subscription.Handler));
               }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
               var subscription = toRemove[i];
               target.Remove(subscription);
            }
         }

         for (var i = 0; i < toRaise.Count; i++)
         {
            var (subscriber, handler) = toRaise[i];
            handler.Invoke(subscriber, new[] { sender, args });
         }
      }

      /// <summary>
      ///    Removes the event handler.
      /// </summary>
      /// <typeparam name="TEventArgs">The type of the t event arguments.</typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="ArgumentNullException">
      ///    eventName
      ///    or
      ///    handler
      /// </exception>
      public static void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler,
                                                        [CallerMemberName] string eventName = null)
         where TEventArgs : EventArgs
      {
         if (eventName.IsEmpty())
         {
            throw new ArgumentNullException(nameof(eventName));
         }

         if (handler == null)
         {
            throw new ArgumentNullException(nameof(handler));
         }

         RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      /// <summary>
      ///    Removes the event handler.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="ArgumentNullException">
      ///    eventName
      ///    or
      ///    handler
      /// </exception>
      public static void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
      {
         if (eventName.IsEmpty())
         {
            throw new ArgumentNullException(nameof(eventName));
         }

         if (handler == null)
         {
            throw new ArgumentNullException(nameof(handler));
         }

         RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      /// <summary>
      ///    Adds the event handler.
      /// </summary>
      /// <param name="eventName">Name of the event.</param>
      /// <param name="handlerTarget">The handler target.</param>
      /// <param name="methodInfo">The method information.</param>
      private static void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
      {
         if (!_eventHandlers.TryGetValue(eventName, out var targets))
         {
            targets = new List<Subscription>();
            _eventHandlers.Add(eventName, targets);
         }

         if (handlerTarget == null)
         {
            // This event handler is a static method
            targets.Add(new Subscription(null, methodInfo));
            return;
         }

         targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
      }

      /// <summary>
      ///    Removes the event handler.
      /// </summary>
      /// <param name="eventName">Name of the event.</param>
      /// <param name="handlerTarget">The handler target.</param>
      /// <param name="methodInfo">The method information.</param>
      private static void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
      {
         if (!_eventHandlers.TryGetValue(eventName, out var subscriptions))
         {
            return;
         }

         for (var n = subscriptions.Count; n > 0; n--)
         {
            var current = subscriptions[n - 1];

            if (current.Subscriber?.Target != handlerTarget || current.Handler.Name != methodInfo.Name)
            {
               continue;
            }

            subscriptions.Remove(current);
            break;
         }
      }

      /// <summary>
      ///    Struct Subscription
      /// </summary>
      private struct Subscription
      {
         /// <summary>
         ///    The handler
         /// </summary>
         public readonly MethodInfo Handler;

         /// <summary>
         ///    The subscriber
         /// </summary>
         public readonly WeakReference Subscriber;

         /// <summary>
         ///    Initializes a new instance of the <see cref="Subscription" /> struct.
         /// </summary>
         /// <param name="subscriber">The subscriber.</param>
         /// <param name="handler">The handler.</param>
         /// <exception cref="ArgumentNullException">handler</exception>
         public Subscription(WeakReference subscriber, MethodInfo handler)
         {
            Subscriber = subscriber;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
         }
      }
   }
}
