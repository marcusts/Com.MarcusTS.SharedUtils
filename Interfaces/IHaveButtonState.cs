// *********************************************************************************
// <copyright file=IHaveButtonState.cs company="Marcus Technical Services, Inc.">
//     Copyright @2019 Marcus Technical Services, Inc.
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

namespace Com.MarcusTS.SharedUtils.Interfaces
{
   using Com.MarcusTS.SharedUtils.Utils;
   using System;

   /// <summary>
   ///    Enum ButtonStates
   /// </summary>
   public enum ButtonStates
   {
      // Important to have this first
      /// <summary>
      ///    The deselected
      /// </summary>
      Deselected,

      /// <summary>
      ///    The selected
      /// </summary>
      Selected,

      /// <summary>
      ///    The disabled
      /// </summary>
      Disabled
   }

   /// <summary>
   ///    Interface IHaveButtonState
   /// </summary>
   public interface IHaveButtonState
   {
      /// <summary>
      ///    Gets or sets the state of the button.
      /// </summary>
      /// <value>The state of the button.</value>
      ButtonStates ButtonState { get; set; }

      /// <summary>
      ///    Gets or sets the selection group.
      /// </summary>
      /// <value>The selection group.</value>
      int SelectionGroup { get; set; }

      /// <summary>
      ///    Occurs when [button state changed].
      /// </summary>
      event EventHandler<ButtonStates> ButtonStateChanged;

      /// <summary>
      ///    Occurs when [view button pressed].
      /// </summary>
      event EventUtils.NoParamsDelegate ViewButtonPressed;
   }
}