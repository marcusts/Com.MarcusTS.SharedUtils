// #define AVOID_CONTEXT_MANAGEMENT

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Diagnostics;
   using System.Threading.Tasks;
   using Xamarin.Forms;

   /// <summary>
   ///    Class ThreadHelper.
   /// </summary>
   public static class ThreadHelper
   {
      /// <summary>
      ///    Gets a value indicating whether this instance is on main thread.
      /// </summary>
      /// <value><c>true</c> if this instance is on main thread; otherwise, <c>false</c>.</value>
      public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

      /// <summary>
      ///    Gets the main thread identifier.
      /// </summary>
      /// <value>The main thread identifier.</value>
      public static int MainThreadId { get; private set; }

      /// <summary>
      ///    Initializes the specified main thread identifier.
      /// </summary>
      /// <param name="mainThreadId">The main thread identifier.</param>
      public static void Initialize(int mainThreadId)
      {
         MainThreadId = mainThreadId;
      }


      /// <summary>Runs a Task without changing the context (configure await is false).</summary>
      /// <param name="task">The task.</param>
      /// <returns>Task.</returns>
      public static Task WithoutChangingContext(this Task task)
      {
#if !AVOID_CONTEXT_MANAGEMENT
         task.ConfigureAwait(false);
#endif
         return task;
      }

      /// <summary>Runs a Task without changing the context (configure await is false).</summary>
      /// <param name="task">The task.</param>
      /// <returns>Task&lt;T&gt;.</returns>
      public static Task<T> WithoutChangingContext<T>(this Task<T> task)
      {
#if !AVOID_CONTEXT_MANAGEMENT
         task.ConfigureAwait(false);
#endif
         return task;
      }

      /// <remarks>
      /// https://johnthiriet.com/removing-async-void/
      /// {With modifications}
      /// </remarks>
      public static async void FireAndForget(this Task task, IErrorHandler handler = default)
      {
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
      }

      /// <summary>
      /// Runs the parallel.
      /// </summary>
      /// <param name="task">The task.</param>
      /// <param name="taskCallback">The task callback.</param>
      /// <param name="actionCallback">The action callback.</param>
      public static void RunParallel
      (
         this Task task,
         Task      taskCallback   = null,
         Action    actionCallback = null)
      {
         try
         {
            Task.Run
            (
               async () =>
               {
                  await task.WithoutChangingContext();

                  if (taskCallback != null)
                  {
                     Device.BeginInvokeOnMainThread
                     (
                        async () => { await taskCallback.WithoutChangingContext(); }
                     );
                  }

                  if (actionCallback != null)
                  {
                     Device.BeginInvokeOnMainThread
                     (
                        actionCallback.Invoke
                     );
                  }
               }
            );
         }
         catch (Exception ex)
         {
            Debug.WriteLine(nameof(RunParallel) + " error ->" + ex.Message + "<-");
         }
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
}
