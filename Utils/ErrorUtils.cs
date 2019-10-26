#region License

// Copyright (c) 2019  Marcus Technical Services, Inc. <marcus@marcusts.com>
//
// This file, ErrorUtils.cs, is a part of a program called AccountViewMobile.
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

   /// <summary>
   /// Class ErrorUtils.
   /// </summary>
   public static class ErrorUtils
   {
      /// <summary>
      /// Considers the argument error.
      /// </summary>
      /// <param name="condition">if set to <c>true</c> [condition].</param>
      /// <param name="message">The message.</param>
      public static void ConsiderArgumentError(bool condition, string message)
      {
         if (condition)
         {
            ThrowArgumentError(message);
         }
      }

      /// <summary>
      /// Throws the argument error.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <exception cref="ArgumentException"></exception>
      public static void ThrowArgumentError(string message)
      {
         throw new ArgumentException(message);
      }
   }
}