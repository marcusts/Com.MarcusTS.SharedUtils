#region License

// Copyright (c) 2019  Marcus Technical Services, Inc. <marcus@marcusts.com>
//
// This file, EventUtils.cs, is a part of a program called AccountViewMobile.
//
// AccountViewMobile is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Permission to use, copy, modify, and/or distribute this software
// for any purpose with or without fee is hereby granted, provided
// that the above copyright notice and this permission notice appear
// in all copies.
//
// AccountViewMobile is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// For the complete GNU General Public License,
// see <http://www.gnu.org/licenses/>.

#endregion

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Threading;
   using System.Threading.Tasks;

   /// <summary>
   /// Class EventUtils.
   /// </summary>
   public static class EventUtils
   {
      /// <summary>
      /// Delegate GenericDelegate
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="val">The value.</param>
      public delegate void GenericDelegate<in T>(T val);

      /// <summary>
      /// Delegate GenericDelegateTask
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="val">The value.</param>
      /// <returns>Task.</returns>
      public delegate Task GenericDelegateTask<in T>(T val);

      /// <summary>
      /// Delegate NoParamsDelegate
      /// </summary>
      public delegate void NoParamsDelegate();

      /// <summary>
      /// Delegate NoParamsDelegateTask
      /// </summary>
      /// <returns>Task.</returns>
      public delegate Task NoParamsDelegateTask();

      /// <summary>
      /// Raises the specified sender.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The e.</param>
      public static void Raise<T>(this EventHandler<T> handler,
                                  object               sender,
                                  T                    e) where T : EventArgs
      {
         handler?.Invoke(sender, e);
      }

      /// <summary>
      /// Raises the specified sender.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      public static void Raise(this EventHandler handler,
                               object            sender,
                               EventArgs         e)
      {
         handler?.Invoke(sender, e);
      }

      /// <summary>
      /// Raises the on different thread.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The e.</param>
      public static void RaiseOnDifferentThread<T>(this EventHandler<T> handler,
                                                   object               sender,
                                                   T                    e) where T : EventArgs
      {
         if (handler != null)
         {
            Task.Factory.StartNewOnDifferentThread(() => handler.Raise(sender, e));
         }
      }

      /// <summary>
      /// Raises the on different thread.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      public static void RaiseOnDifferentThread(this EventHandler handler,
                                                object            sender,
                                                EventArgs         e)
      {
         if (handler != null)
         {
            Task.Factory.StartNewOnDifferentThread(() => handler.Raise(sender, e));
         }
      }

      /// <summary>
      /// Starts the new on different thread.
      /// </summary>
      /// <param name="taskFactory">The task factory.</param>
      /// <param name="action">The action.</param>
      /// <returns>Task.</returns>
      public static Task StartNewOnDifferentThread(this TaskFactory taskFactory,
                                                   Action           action)
      {
         return taskFactory.StartNew(action, new CancellationToken());
      }
   }
}