// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=CustomWeakEventManager.cs
// company="Marcus Technical Services, Inc.">
// </copyright>
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

namespace Com.MarcusTS.SharedUtils.Events
{
   using System;
   using System.Collections.Generic;
   using System.Reflection;
   using System.Runtime.CompilerServices;
   using Com.MarcusTS.SharedUtils.Utils;

   /// <summary>
   /// Copied from (internal) Xamarin.Forms to make it publicly accessible.
   /// </summary>
   public static class CustomWeakEventManager
   {
      /// <summary>
      /// The event handlers
      /// </summary>
      private static readonly Dictionary<string, List<Subscription>> _eventHandlers =
         new Dictionary<string, List<Subscription>>();

      /// <summary>
      /// Adds the event handler.
      /// </summary>
      /// <typeparam name="TEventArgs">The type of the t event arguments.</typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="System.ArgumentNullException">eventName</exception>
      /// <exception cref="System.ArgumentNullException">handler</exception>
      /// <exception cref="ArgumentNullException">eventName</exception>
      public static void AddEventHandler<TEventArgs>(EventHandler<TEventArgs>  handler,
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
      /// Adds the event handler.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="System.ArgumentNullException">eventName</exception>
      /// <exception cref="System.ArgumentNullException">handler</exception>
      /// <exception cref="ArgumentNullException">eventName</exception>
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
      /// Handles the event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The arguments.</param>
      /// <param name="eventName">Name of the event.</param>
      public static void HandleEvent(object sender, object args, string eventName)
      {
         var toRaise  = new List<(object subscriber, MethodInfo handler)>();
         var toRemove = new List<Subscription>();

         if (_eventHandlers.TryGetValue(eventName, out var target))
         {
            foreach (var subscription in target)
            {
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

            foreach (var subscription in toRemove)
            {
               target.Remove(subscription);
            }
         }

         foreach (var t in toRaise)
         {
            (var subscriber, var handler) = t;
            handler.Invoke(subscriber, new[] { sender, args });
         }
      }

      /// <summary>
      /// Removes the event handler.
      /// </summary>
      /// <typeparam name="TEventArgs">The type of the t event arguments.</typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="System.ArgumentNullException">eventName</exception>
      /// <exception cref="System.ArgumentNullException">handler</exception>
      /// <exception cref="ArgumentNullException">eventName</exception>
      public static void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs>  handler,
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
      /// Removes the event handler.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="eventName">Name of the event.</param>
      /// <exception cref="System.ArgumentNullException">eventName</exception>
      /// <exception cref="System.ArgumentNullException">handler</exception>
      /// <exception cref="ArgumentNullException">eventName</exception>
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
      /// Adds the event handler.
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
      /// Removes the event handler.
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

            if ((current.Subscriber?.Target != handlerTarget) || (current.Handler.Name != methodInfo.Name))
            {
               continue;
            }

            subscriptions.Remove(current);
            break;
         }
      }

      /// <summary>
      /// Struct Subscription
      /// </summary>
      private readonly struct Subscription
      {
         /// <summary>
         /// The handler
         /// </summary>
         public readonly MethodInfo Handler;

         /// <summary>
         /// The subscriber
         /// </summary>
         public readonly WeakReference Subscriber;

         /// <summary>
         /// Initializes a new instance of the <see cref="Subscription" /> struct.
         /// </summary>
         /// <param name="subscriber">The subscriber.</param>
         /// <param name="handler">The handler.</param>
         /// <exception cref="System.ArgumentNullException">handler</exception>
         /// <exception cref="ArgumentNullException">handler</exception>
         public Subscription(WeakReference subscriber, MethodInfo handler)
         {
            Subscriber = subscriber;
            Handler    = handler ?? throw new ArgumentNullException(nameof(handler));
         }
      }
   }
}