﻿// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ThreadHelper.cs
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

// #define AVOID_CONTEXT_MANAGEMENT
#define FORCE_WAIT_FROM_VOID

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Diagnostics;
   using System.Runtime.CompilerServices;
   using System.Threading.Tasks;
   using Com.MarcusTS.SharedUtils.Interfaces;

   /// <summary></summary>
   public interface IErrorHandler
   {
      /// <summary></summary>
      /// <param name="ex"></param>
      void HandleError(Exception ex);
   }

   public interface IRunnableTask : ICanRun
   {
      Task TaskToRun { get; }
   }

   /// <summary>Class ThreadHelper.</summary>
   public static class ThreadHelper
   {
      public const int DEFAULT_TIMER_MILLISECONDS = 100;

      /// <summary>Gets a value indicating whether this instance is on main thread.</summary>
      /// <value><c>true</c> if this instance is on main thread; otherwise, <c>false</c>.</value>
      [Obsolete]
      public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

      /// <summary>Gets the main thread identifier.</summary>
      /// <value>The main thread identifier.</value>
      [Obsolete]
      public static int MainThreadId { get; private set; }

      /// <remarks>https://johnthiriet.com/removing-async-void/ {With modifications}</remarks>
      public static
#if !FORCE_WAIT_FROM_VOID
         async
#endif
         void FireAndForget(this Task task, IErrorHandler handler = default)
      {
#if FORCE_WAIT_FROM_VOID

         // Boolean response is ignored
         WaitFromVoid(task);

#else
         try
         {
            await task.WithoutChangingContext();
         }
         catch (Exception ex)
         {
            if (handler.IsNullOrDefault())
            {
               handler = new DefaultErrorHandler();
            }

            handler?.HandleError(ex);
         }

#endif
      }

      /// <summary>Initializes the specified main thread identifier.</summary>
      /// <param name="mainThreadId">The main thread identifier.</param>
      [Obsolete]
      public static void Initialize(int mainThreadId)
      {
         MainThreadId = mainThreadId;
      }

      /// <summary>Replaces FireAndForget with a true "wait" while loop along with timeout</summary>
      public static void WaitFromVoid(
         this Task taskToRun,
         int       timerMilliseconds = DEFAULT_TIMER_MILLISECONDS)
      {
         IRunnableTask parentTask = new RunnableTask(taskToRun);

         parentTask.TaskToRun.WaitFromVoid(parentTask, timerMilliseconds);
      }

      /// <summary>Runs a Task without changing the context (configure await is false).</summary>
      /// <param name="task">The task.</param>
      /// <returns>Task.</returns>
      public static ConfiguredTaskAwaitable WithoutChangingContext(this Task task)
      {
#if AVOID_CONTEXT_MANAGEMENT
         return task.ConfigureAwait(true);
#else
         return task.ConfigureAwait(false);
#endif
      }

      /// <summary>Runs a Task without changing the context (configure await is false).</summary>
      /// <param name="task">The task.</param>
      /// <returns>Task&lt;T&gt;.</returns>
      public static ConfiguredTaskAwaitable<T> WithoutChangingContext<T>(this Task<T> task)
      {
#if AVOID_CONTEXT_MANAGEMENT
         return task.ConfigureAwait(true);
#endif
         return task.ConfigureAwait(false);
      }

      /// <summary>WORKS ONLY WITH RUNNABLE TASK THAT SETS IS RUNNING INTERNALLY (SEE BELOW)</summary>
      private static void WaitFromVoid(
         this Task     taskToRun,
         IRunnableTask parentTask,
         int           timerMilliseconds = DEFAULT_TIMER_MILLISECONDS)
      {
         if (parentTask.IsNullOrDefault() || taskToRun.IsNullOrDefault())
         {
            return;
         }

         // ELSE
         new Action(async () =>
            {
               // Run on a thread
               taskToRun.RunParallel();

               // Wait for the thread
               // ReSharper disable once PossibleNullReferenceException
               while (parentTask.IsRunning.IsTrue())
               {
                  await Task.Delay(timerMilliseconds).WithoutChangingContext();
               }
            })
           .Invoke();
      }
   }

   /// <summary></summary>
   public class DefaultErrorHandler : IErrorHandler
   {
      /// <summary></summary>
      /// <param name="ex"></param>
      public void HandleError(Exception ex)
      {
         Debug.WriteLine(ex.Message);
      }
   }

   public class RunnableTask : IRunnableTask
   {
      public RunnableTask(Task task)
      {
         TaskToRun = task.ContinueWith(t => IsRunning.SetFalse());
      }

      // Set true by default
      public IThreadSafeAccessor IsRunning { get; } = new ThreadSafeAccessor(1);

      public Task TaskToRun { get; }
   }
}
