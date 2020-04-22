namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

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
   }
}