// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=IMenuNavigationState.cs
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

namespace Com.MarcusTS.SharedUtils.Interfaces
{
   /// <summary>
   /// A view model base for pages.
   /// </summary>
   public interface IMenuNavigationState
   {
      /// <summary>
      /// The app state to navigate to
      /// </summary>
      /// <value>The state of the application.</value>
      string AppState { get; }

      /// <summary>
      /// Gets the menu order.
      /// </summary>
      /// <value>The menu order.</value>
      int MenuOrder { get; }

      /// <summary>
      /// The menu title
      /// </summary>
      /// <value>The menu title.</value>
      string MenuTitle { get; }

      /// <summary>
      /// The page or view title (default: centered on the page)
      /// </summary>
      /// <value>The view title.</value>
      string ViewTitle { get; }
   }
}