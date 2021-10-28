// *********************************************************************************
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
// #define FORCE_ALL_BEGIN_INVOKE

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Diagnostics;
   using System.Runtime.CompilerServices;
   using System.Threading;
   using System.Threading.Tasks;
   using Com.MarcusTS.SharedUtils.Interfaces;
   using Xamarin.Essentials;

   /// <summary>
   /// Class ThreadHelper.
   /// </summary>
   public static class ThreadHelper
   {
      public const int MAX_DELAY_MILLISECONDS = 10000;

      /// <summary>
      /// The default timer milliseconds
      /// </summary>
      public const int MILLISECONDS_BETWEEN_DELAYS = 25;

      /// <summary>
      /// Gets a value indicating whether this instance is on main thread.
      /// </summary>
      /// <value><c>true</c> if this instance is on main thread; otherwise, <c>false</c>.</value>
      [Obsolete]
      public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

      /// <summary>
      /// Gets the main thread identifier.
      /// </summary>
      /// <value>The main thread identifier.</value>
      [Obsolete]
      public static int MainThreadId { get; private set; }

      public static void ConsiderBeginInvokeActionOnMainThread
      (
         Action action,
         bool forceBeginInvoke =
#if FORCE_ALL_BEGIN_INVOKE
            true
#else
            false
#endif
      )
      {
         if ( action.IsNullOrDefault() )
         {
            return;
         }

         // ELSE a valid action
         if ( !forceBeginInvoke && MainThread.IsMainThread )
         {
            action.Invoke();
         }
         else
         {
            MainThread.BeginInvokeOnMainThread( action.Invoke );
         }
      }

      public static async Task ConsiderBeginInvokeTaskOnMainThread
      (
         this Task task,
         bool forceBeginInvoke =
#if FORCE_ALL_BEGIN_INVOKE
         true
#else
            false
#endif
      )
      {
         if ( !forceBeginInvoke && MainThread.IsMainThread )
         {
            await task.WithoutChangingContext();
         }
         else
         {
            MainThread.BeginInvokeOnMainThread( () => { task.FireAndFuhgetAboutIt(); } );
         }
      }

      /// <summary>
      /// Moving over to the uniquely-named <see cref="FireAndFuhgetAboutIt" />. This method is available from many other
      /// sources.
      /// </summary>
      /// <remarks>https://johnthiriet.com/removing-async-void/ {With modifications}</remarks>
      [Obsolete]

      // ReSharper disable once AsyncVoidMethod
      public static async void FireAndForget( this Task task, IErrorHandler handler = default )
      {
         try
         {
            await task.WithoutChangingContext();
         }
         catch ( Exception ex )
         {
            if ( handler.IsNullOrDefault() )
            {
               handler = new DefaultErrorHandler();
            }

            handler?.HandleError( ex );
         }
      }

      /// <summary>
      /// Now calls WaitFromVoid rather than FireAndForget.
      /// </summary>
      public static void FireAndFuhgetAboutIt( this Task task, IErrorHandler handler = default )
      {
         // Boolean response is ignored
         WaitFromVoid( task );
      }

      /// <summary>
      /// Initializes the specified main thread identifier.
      /// </summary>
      /// <param name="mainThreadId">The main thread identifier.</param>
      [Obsolete]
      public static void Initialize( int mainThreadId )
      {
         MainThreadId = mainThreadId;
      }

      /// <summary>
      /// Runs the task in parallel unconditionally and without waiting.
      /// </summary>
      /// <param name="task">The task.</param>
      public static void RunParallel
      (
         this Task task
      )
      {
         try
         {
            _ = Task.Run
            (

               // IMPORTANT The task will run without awaiting the foreground thread and without coordination with other concurrent calls.
               async () => await task.WithoutChangingContext()
            );
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( nameof( RunParallel ) + " error ->" + ex.Message + "<-" );
         }
      }

      /// <summary>
      /// Replaces FireAndFuhgetAboutIt with a true "wait" while loop along with timeout
      /// </summary>
      /// <param name="taskToRun">The task to run.</param>
      /// <param name="delayMilliseconds"></param>
      /// <param name="maxWaitMilliseconds"></param>
      /// <param name="cancelTokenSource"></param>
      public static void WaitFromVoid(
         this Task               taskToRun,
         int                     delayMilliseconds   = MILLISECONDS_BETWEEN_DELAYS,
         int                     maxWaitMilliseconds = MAX_DELAY_MILLISECONDS,
         CancellationTokenSource cancelTokenSource   = default )
      {
         IRunnableTask parentTask = new RunnableTask( taskToRun );

         parentTask.TaskToRun.WaitFromVoid( parentTask, delayMilliseconds, maxWaitMilliseconds, cancelTokenSource );
      }

      /// <summary>
      /// Runs a Task without changing the context (configure await is false).
      /// </summary>
      /// <param name="task">The task.</param>
      /// <returns>Task.</returns>
      public static ConfiguredTaskAwaitable WithoutChangingContext( this Task task )
      {
#if AVOID_CONTEXT_MANAGEMENT
         return task.ConfigureAwait(true);
#else
         return task.ConfigureAwait( false );
#endif
      }

      /// <summary>
      /// Runs a Task without changing the context (configure await is false).
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="task">The task.</param>
      /// <returns>Task&lt;T&gt;.</returns>
      public static ConfiguredTaskAwaitable<T> WithoutChangingContext<T>( this Task<T> task )
      {
#if AVOID_CONTEXT_MANAGEMENT
         return task.ConfigureAwait(true);
#endif
         return task.ConfigureAwait( false );
      }

      /// <param name="maxDelay"></param>
      private static CancellationTokenSource CreateCancellationTokenSource( int maxDelay )
      {
         var cancellationTokenSource = new CancellationTokenSource();
         cancellationTokenSource.CancelAfter( TimeSpan.FromMilliseconds( maxDelay ) );
         return cancellationTokenSource;
      }

      /// <summary>
      /// WORKS ONLY WITH RUNNABLE TASK THAT SETS IS RUNNING INTERNALLY (SEE BELOW)
      /// </summary>
      /// <param name="taskToRun">The task to run.</param>
      /// <param name="parentTask">The parent task.</param>
      /// <param name="delayMilliseconds">The timer milliseconds.</param>
      /// <param name="maxWaitMilliseconds"></param>
      /// <param name="cancelTokenSource"></param>
      // ReSharper disable once AsyncVoidMethod
      private static async void WaitFromVoid(
         this Task               taskToRun,
         IRunnableTask           parentTask,
         int                     delayMilliseconds   = MILLISECONDS_BETWEEN_DELAYS,
         int                     maxWaitMilliseconds = MAX_DELAY_MILLISECONDS,
         CancellationTokenSource cancelTokenSource   = default
      )
      {
         if ( parentTask.IsNullOrDefault() || taskToRun.IsNullOrDefault() )
         {
            return;
         }

         if ( cancelTokenSource.IsNullOrDefault() )
         {
            cancelTokenSource = CreateCancellationTokenSource( maxWaitMilliseconds );
         }

         // ELSE
         // Run on a thread
         try
         {
            _ = Task.Run
            (

               // IMPORTANT The task will run without awaiting the foreground thread and without coordination with other concurrent calls.
               async () => await taskToRun.WithoutChangingContext()
            );

            // Wait for the thread
            // ReSharper disable once PossibleNullReferenceException
            while ( !cancelTokenSource.Token.IsCancellationRequested && parentTask.IsRunning.IsTrue() )
            {
               // IMPORTANT This is a legal await from void and on the foreground thread because it occurs in a "while" loop and is also tried and caught.
               await Task.Delay( delayMilliseconds ).WithoutChangingContext();
            }
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( nameof( WaitFromVoid ) + " error ->" + ex.Message + "<-" );
         }
      }
   }

   public class DefaultErrorHandler : IErrorHandler
   {
      /// <summary>
      /// Handles the error.
      /// </summary>
      /// <param name="ex">The ex.</param>
      public void HandleError( Exception ex )
      {
         Debug.WriteLine( ex.Message );
      }
   }

   public class RunnableTask : IRunnableTask
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RunnableTask" /> class.
      /// </summary>
      /// <param name="task">The task.</param>
      public RunnableTask( Task task )
      {
         TaskToRun = task.ContinueWith( t => IsRunning.SetFalse() );
      }

      // Set true by default
      /// <summary>
      /// Gets the is running.
      /// </summary>
      /// <value>The is running.</value>
      public IThreadSafeAccessor IsRunning { get; } = new ThreadSafeAccessor( 1 );

      /// <summary>
      /// Gets the task to run.
      /// </summary>
      /// <value>The task to run.</value>
      public Task TaskToRun { get; }
   }
}