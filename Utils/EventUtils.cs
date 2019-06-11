// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 01-02-2019
//
// <copyright file="EventUtils.cs" company="Marcus Technical Services, Inc.">
//     @2018 Marcus Technical Services, Inc.
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

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Threading;
   using System.Threading.Tasks;

   public static class EventUtils
   {
      #region Public Delegates

      public delegate void GenericDelegate<in T>(T val);

      public delegate Task GenericDelegateTask<in T>(T val);

      public delegate void NoParamsDelegate();

      public delegate Task NoParamsDelegateTask();

      #endregion Public Delegates

      #region Public Methods

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
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

      #endregion Public Methods
   }
}
