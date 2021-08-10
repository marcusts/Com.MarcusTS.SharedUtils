// #define AVOID_CONTEXT_MANAGEMENT
#define FORCE_WAIT_FROM_VOID

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
   ///    Class ThreadHelper.
   /// </summary>
   public static class ThreadHelper
   {
      /// <summary>
      ///    Gets a value indicating whether this instance is on main thread.
      /// </summary>
      /// <value><c>true</c> if this instance is on main thread; otherwise, <c>false</c>.</value>
      [Obsolete]
      public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

      /// <summary>
      ///    Gets the main thread identifier.
      /// </summary>
      /// <value>The main thread identifier.</value>
      [Obsolete]
      public static int MainThreadId { get; private set; }

      /// <summary>
      ///    Initializes the specified main thread identifier.
      /// </summary>
      /// <param name="mainThreadId">The main thread identifier.</param>
      [Obsolete]
      public static void Initialize(int mainThreadId)
      {
         MainThreadId = mainThreadId;
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

      /// <remarks>
      /// https://johnthiriet.com/removing-async-void/
      /// {With modifications}
      /// </remarks>
      public static 

#if !FORCE_WAIT_FROM_VOID
         async 
#endif

         void FireAndForget(this Task task, IErrorHandler handler = default)
      {

#if FORCE_WAIT_FROM_VOID

         WaitFromVoid( task);

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

      public const int DEFAULT_TIMER_MILLISECONDS =  100;

      /*
      /// <summary>
      /// Replaces FireAndForget with a true "wait" while loop along with timeout
      /// </summary>
      /// <param name="task"></param>
      /// <param name="timerMilliseconds"></param>
      public static void WaitFromVoid( this ICanRun task, int timerMilliseconds = DEFAULT_TIMER_MILLISECONDS )
      {
         if (task.IsNullOrDefault())
         {
            return;
         }

         new Action(async () =>
            {
               // Run on a thread
               task.RunParallel();

               // Wait for the thread
               while (!task.IsCompleted)
               {
                  await Task.Delay(timerMilliseconds).WithoutChangingContext();
               }
            })
            .Invoke();
      }
      */

      /// <summary>
      /// Replaces FireAndForget with a true "wait" while loop along with timeout
      /// </summary>
      public static void WaitFromVoid( 
         this Task taskToRun, 
         ICanRun parentTask, 
         int timerMilliseconds = DEFAULT_TIMER_MILLISECONDS )
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
               while (parentTask.IsRunning.IsTrue())
               {
                  await Task.Delay(timerMilliseconds).WithoutChangingContext();
               }
            })
            .Invoke();
      }

      /// <summary>
      /// Replaces FireAndForget with a true "wait" while loop along with timeout
      /// </summary>
      public static void WaitFromVoid( 
         this Task taskToRun, 
         int timerMilliseconds = DEFAULT_TIMER_MILLISECONDS )
      {
         IRunnableTask parentTask = new RunnableTask(taskToRun);

         parentTask.TaskToRun.WaitFromVoid(parentTask, timerMilliseconds);
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public interface IErrorHandler
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="ex"></param>
      void HandleError(Exception ex);
   }

   /// <summary>
   /// 
   /// </summary>
   public class DefaultErrorHandler : IErrorHandler
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="ex"></param>
      public void HandleError(Exception ex)
      {
         Debug.WriteLine(ex.Message);
      }
   }

   public interface IRunnableTask : ICanRun
   {
      Task TaskToRun { get; }
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
