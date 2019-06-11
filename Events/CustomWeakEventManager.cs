// *********************************************************************************
// Assembly         : Com.MarcusTS.SharedUtils
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 01-03-2019
// Last Modified On : 01-03-2019
//
// <copyright file="CopiedWeakEventManager.cs" company="Marcus Technical Services, Inc.">
//     Copyright @2018 Marcus Technical Services, Inc.
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
   using Utils;

   /// <summary>
   /// Copied from (internal) Xamarin.Forms  to make it publicly accessible.
   /// </summary>
   public static class CustomWeakEventManager
   {
      private static readonly Dictionary<string, List<Subscription>> _eventHandlers = new Dictionary<string, List<Subscription>>();

      public static void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName]string eventName = null)
         where TEventArgs : EventArgs
      {
         if (eventName.IsEmpty())
            throw new ArgumentNullException(nameof(eventName));

         if (handler == null)
            throw new ArgumentNullException(nameof(handler));

         AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      public static void AddEventHandler(EventHandler handler, [CallerMemberName]string eventName = null)
      {
         if (eventName.IsEmpty())
            throw new ArgumentNullException(nameof(eventName));

         if (handler == null)
            throw new ArgumentNullException(nameof(handler));

         AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      public static void HandleEvent(object sender, object args, string eventName)
      {
         var toRaise = new List<(object subscriber, MethodInfo handler)>();
         var toRemove = new List<Subscription>();

         if (_eventHandlers.TryGetValue(eventName, out List<Subscription> target))
         {
            for (int i = 0; i < target.Count; i++)
            {
               Subscription subscription = target[i];
               bool isStatic = subscription.Subscriber == null;
               if (isStatic)
               {
                  // For a static method, we'll just pass null as the first parameter of MethodInfo.Invoke
                  toRaise.Add((null, subscription.Handler));
                  continue;
               }

               object subscriber = subscription.Subscriber.Target;

               if (subscriber == null)
                  // The subscriber was collected, so there's no need to keep this subscription around
                  toRemove.Add(subscription);
               else
                  toRaise.Add((subscriber, subscription.Handler));
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
               Subscription subscription = toRemove[i];
               target.Remove(subscription);
            }
         }

         for (int i = 0; i < toRaise.Count; i++)
         {
            (var subscriber, var handler) = toRaise[i];
            handler.Invoke(subscriber, new[] { sender, args });
         }
      }

      public static void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName]string eventName = null)
         where TEventArgs : EventArgs
      {
         if (eventName.IsEmpty())
            throw new ArgumentNullException(nameof(eventName));

         if (handler == null)
            throw new ArgumentNullException(nameof(handler));

         RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      public static void RemoveEventHandler(EventHandler handler, [CallerMemberName]string eventName = null)
      {
         if (eventName.IsEmpty())
            throw new ArgumentNullException(nameof(eventName));

         if (handler == null)
            throw new ArgumentNullException(nameof(handler));

         RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
      }

      private static void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
      {
         if (!_eventHandlers.TryGetValue(eventName, out List<Subscription> targets))
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

      private static void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
      {
         if (!_eventHandlers.TryGetValue(eventName, out List<Subscription> subscriptions))
            return;

         for (int n = subscriptions.Count; n > 0; n--)
         {
            Subscription current = subscriptions[n - 1];

            if (current.Subscriber?.Target != handlerTarget || current.Handler.Name != methodInfo.Name)
               continue;

            subscriptions.Remove(current);
            break;
         }
      }

      struct Subscription
      {
         public Subscription(WeakReference subscriber, MethodInfo handler)
         {
            Subscriber = subscriber;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
         }

         public readonly WeakReference Subscriber;
         public readonly MethodInfo Handler;
      }
   }
}