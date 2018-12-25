// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="IHavePageViewModel.cs" company="Marcus Technical Services, Inc.">
//     @2018 Marcus Technical Services, Inc.
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
   /// The HasViewModel interface. Implements a ViewModel of type T. T must be an IAmBusy. This is
   /// basically a marker interface for some kind of object that at least has a ViewModel. Used for IoC.
   /// </summary>
   /// <typeparam name="T">the type of ViewModel</typeparam>
   public interface IHavePageViewModel<out T> where T : class
   {
      #region Public Properties

      /// <summary>
      /// Gets the view model.
      /// </summary>
      /// <value>The view model.</value>
      T ViewModel { get; }

      #endregion Public Properties
   }
}