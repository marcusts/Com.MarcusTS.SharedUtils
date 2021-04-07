namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

   /// <summary>
   ///    Class ErrorUtils.
   /// </summary>
   public static class ErrorUtils
   {
      /// <summary>
      /// Confusing function; use <see cref="IssueArgumentErrorIfFalse"/> or <see cref="IssueArgumentErrorIfTrue"/>.
      /// </summary>
      /// <param name="condition">if set to <c>true</c> [condition].</param>
      /// <param name="message">The message.</param>
      [Obsolete]
      public static void ConsiderArgumentError(bool condition, string message)
      {
         if (condition)
         {
            ThrowArgumentError(message);
         }
      }

      /// <summary>
      /// Issues an argument error if the condition is false.
      /// </summary>
      /// <param name="condition"></param>
      /// <param name="message"></param>
      public static void IssueArgumentErrorIfFalse(bool condition, string message)
      {
         if (!condition)
         {
            ThrowArgumentError(message);
         }
      }

      /// <summary>
      /// Issues an argument error if the condition is true.
      /// </summary>
      /// <param name="condition"></param>
      /// <param name="message"></param>
      public static void IssueArgumentErrorIfTrue(bool condition, string message)
      {
         IssueArgumentErrorIfFalse(!condition, message);
      }

      /// <summary>
      ///    Throws the argument error.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <exception cref="ArgumentException"></exception>
      public static void ThrowArgumentError(string message)
      {
         throw new ArgumentException(message);
      }
   }
}
