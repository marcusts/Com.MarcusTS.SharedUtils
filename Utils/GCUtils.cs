namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

   public static class GCUtils
   {
      static void TriggerGC()
      {
         Console.WriteLine("Starting GC.");

         GC.Collect();
         GC.WaitForPendingFinalizers();
         GC.Collect();

         Console.WriteLine("GC finished.");
      }
   }
}