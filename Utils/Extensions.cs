// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=Extensions.cs
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

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Collections;
   using System.Collections.Concurrent;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using System.Reflection;
   using System.Text.RegularExpressions;

   /// <summary>
   /// Class Extensions.
   /// </summary>
   public static class Extensions
   {
      /// <summary>
      /// The decimal
      /// </summary>
      public const string DECIMAL = ".";

      /// <summary>
      /// The zero character
      /// </summary>
      public const char ZERO_CHAR = '0';

      /// <summary>
      /// The numeric error
      /// </summary>
      private const double NUMERIC_ERROR = 0.001;

      /// <summary>
      /// Not working consistently; use localized randoms (one per thread)
      /// </summary>
      [Obsolete] public static readonly Random GLOBAL_RANDOM = CreateRandom;

      private static readonly BindingFlags PROP_FLAGS =
         BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

      /// <summary>
      /// The global random
      /// </summary>
      public static Random CreateRandom =>

         // new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
         new Random( Guid.NewGuid().GetHashCode() );

      /// <summary>
      /// Gets a value indicating whether [empty nullable bool].
      /// </summary>
      /// <value>
      /// <c>null</c> if [empty nullable bool] contains no value, <c>true</c> if [empty nullable bool]; otherwise,
      /// <c>false</c>.
      /// </value>
      public static bool? EmptyNullableBool => new bool?();

      /// <summary>
      /// Adds the or update.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <typeparam name="U"></typeparam>
      /// <param name="retDict">The ret dictionary.</param>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public static void AddOrUpdate<T, U>
      (
         this ConcurrentDictionary<T, U> retDict,
         T                               key,
         U                               value
      )
      {
         retDict.AddOrUpdate( key, value,
            (
               k,
               v
            ) => v );
      }

      /// <summary>
      /// Determines if two collections of properties contain the same actual values. Can be called for a single
      /// property using the optional parameter.
      /// </summary>
      /// <typeparam name="T">The type of class which is being evaluated for changes.</typeparam>
      /// <param name="mainViewModel">The main class for comparison.</param>
      /// <param name="otherViewModel">The secondary class for comparison.</param>
      /// <param name="singlePropertyName">If set, this will be the only property interrogated.</param>
      /// <param name="propInfos">The property info collection that will be analyzed for changes.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool AnySettablePropertyHasChanged<T>
      (
         this T         mainViewModel,
         T              otherViewModel,
         string         singlePropertyName,
         PropertyInfo[] propInfos
      )
         where T : class
      {
         if ( mainViewModel.IsNullOrDefault() )
         {
            return false;
         }

         var isChanged = false;

         foreach ( var propInfo in propInfos )
         {
            // If we have a single property, only allow that one through.
            if ( singlePropertyName.IsNotEmpty() && propInfo.Name.IsDifferentThan( singlePropertyName ) )
            {
               continue;
            }

            var currentValue = propInfo.GetValue( mainViewModel );
            var otherValue   = propInfo.GetValue( otherViewModel );

            if ( currentValue.IsNotAnEqualObjectTo( otherValue ) )
            {
               isChanged = true;
               break;
            }
         }

         return isChanged;
      }

      /// <summary>
      /// Appends the array.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="existingArray">The existing array.</param>
      /// <param name="arrayToAppend">The array to append.</param>
      /// <returns>T[].</returns>
      public static T[] AppendArray<T>( this T[] existingArray, T[] arrayToAppend )
      {
         var retList = existingArray.ToList();
         retList.AddRange( arrayToAppend );
         return retList.ToArray();
      }

      /// <summary>
      /// Cleans up service error text.
      /// </summary>
      /// <param name="errorText">The error text.</param>
      /// <returns>System.String.</returns>
      public static string CleanUpServiceErrorText( this string errorText )
      {
         // Find the LAST colon in the string
         var colonPos = errorText.LastIndexOf( ":", StringComparison.CurrentCultureIgnoreCase );
         if ( colonPos > 0 )
         {
            var newErrorText = errorText.Substring( colonPos + 1 );
            newErrorText = newErrorText.Trim( '{' );
            newErrorText = newErrorText.Trim( '}' );
            newErrorText = newErrorText.Trim( '"' );

            return newErrorText;
         }

         return string.Empty;
      }

      /// <summary>
      /// Coerces property values between two interfaces using rules.
      /// </summary>
      /// <typeparam name="TargetT">The type of the target t.</typeparam>
      /// <typeparam name="SourceT">The type of the source t.</typeparam>
      /// <param name="target">The target view model ("this")</param>
      /// <param name="source">The source view model.</param>
      /// <param name="coercePropertyFunc">Changes the propety name (as needed) so it can be found in the target</param>
      /// <param name="targetPropInfos">The property info collection for the target view model.</param>
      /// <param name="sourcePropInfos">The property info collection for the source view model.</param>
      public static void CoerceSettablePropertyValuesFrom<TargetT, SourceT>
      (
         this TargetT                                        target,
         SourceT                                             source,
         Func<PropertyInfo, SourceT, (bool, string, object)> coercePropertyFunc = default,
         PropertyInfo[]                                      targetPropInfos    = default,
         PropertyInfo[]                                      sourcePropInfos    = default
      )
      {
         if ( sourcePropInfos.IsAnEmptyList() )
         {
            sourcePropInfos = typeof( SourceT ).GetRuntimeWriteableProperties();

            if ( sourcePropInfos.IsAnEmptyList() )
            {
               Debug.WriteLine( nameof( CoerceSettablePropertyValuesFrom )      +
                                ": no customized source properties for type ->" + typeof( SourceT ) + "<-" );
               return;
            }
         }

         if ( targetPropInfos.IsAnEmptyList() )
         {
            targetPropInfos = typeof( TargetT ).GetRuntimeWriteableProperties();

            if ( targetPropInfos.IsAnEmptyList() )
            {
               Debug.WriteLine( nameof( CoerceSettablePropertyValuesFrom )      +
                                ": no customized target properties for type ->" + typeof( TargetT ) + "<-" );
               return;
            }
         }

         try
         {
            // ReSharper disable once PossibleNullReferenceException
            foreach ( var sourcePropInfo in sourcePropInfos )
            {
               var includeIt = true;
               var propName  = sourcePropInfo.Name;
               var propValue = sourcePropInfo.GetValue( source );

               if ( coercePropertyFunc.IsNotNullOrDefault() )
               {
                  ( includeIt, propName, propValue ) =

                     // ReSharper disable once PossibleNullReferenceException
                     coercePropertyFunc( sourcePropInfo, source );
               }

               // Make sure the property was found and we can proceed
               if ( includeIt )
               {
                  var foundPropInfo =
                     targetPropInfos.FirstOrDefault( pi => pi.Name.IsSameAs( propName ) );

                  if ( foundPropInfo.IsNotNullOrDefault() )
                  {
                     // Assign the value
                     // ReSharper disable once PossibleNullReferenceException
                     foundPropInfo.SetValue( target, propValue );
                  }
               }
            }
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( nameof( CoerceSettablePropertyValuesFrom ) + " ERROR ->" + ex.Message +
                             "<- source type ->" +
                             typeof( SourceT ) + "<- target type ->" + typeof( TargetT ) + "<-" );
         }
      }

      /// <summary>
      /// Copies the enumerable.
      /// </summary>
      /// <typeparam name="InputListInterface">The type of the input list interface.</typeparam>
      /// <typeparam name="InputListClass">The type of the input list class.</typeparam>
      /// <typeparam name="OutputListInterface">The type of the output list interface.</typeparam>
      /// <typeparam name="OutputListClass">The type of the output list class.</typeparam>
      /// <param name="inputList">The input list.</param>
      /// <param name="creator">The creator.</param>
      /// <returns>OutputListInterface[].</returns>
      public static OutputListInterface[] CopyEnumerable<InputListInterface, InputListClass, OutputListInterface,
                                                         OutputListClass>(
         InputListClass[] inputList, Func<OutputListClass> creator )
         where InputListClass : class, InputListInterface
         where OutputListClass : class, OutputListInterface, InputListInterface
      {
         if ( inputList.IsAnEmptyList() )
         {
            return default;
         }

         var propInfos = typeof( InputListClass ).GetRuntimeWriteableProperties().ToArray();

         if ( propInfos.IsAnEmptyList() )
         {
            return default;
         }

         var tempOutputList = new List<OutputListInterface>();

         // The properties should be of the individual list type - not the originating service model type.
         foreach ( var record in inputList )
         {
            var newRecord = creator?.Invoke();
            newRecord.CopySettablePropertyValuesFrom<InputListInterface>( record, propInfos );
            tempOutputList.Add( newRecord );
         }

         return tempOutputList.ToArray();
      }

      /// <summary>
      /// Copy the values from the specified properties from value to target.
      /// </summary>
      /// <typeparam name="T">*Unused* -- required for referencing only.</typeparam>
      /// <param name="targetViewModel">The view model to copy *to*.</param>
      /// <param name="valueViewModel">The view model to copy *from*.</param>
      /// <param name="propInfos">The property info records to use to get and set values.</param>
      public static void CopySettablePropertyValuesFrom<T>
      (
         this T         targetViewModel,
         T              valueViewModel,
         PropertyInfo[] propInfos = null
      )
      {
         if ( ( propInfos == null ) || propInfos.IsAnEmptyList() )
         {
            propInfos = typeof( T ).GetRuntimeWriteableProperties();
         }

         try
         {
            foreach ( var propInfo in propInfos )
            {
               propInfo.SetValue( targetViewModel, propInfo.GetValue( valueViewModel ) );
            }
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( "CopySettablePropertyValuesFrom error ->" + ex.Message + "<-" );
         }
      }

      /// <summary>
      /// Gets all property infos.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns>PropertyInfo[].</returns>
      public static PropertyInfo[] GetAllPropInfos<T>()
      {
         var retInfos = typeof( T ).GetRuntimeWriteableProperties();
         return retInfos.ToArray();
      }

      /// <summary>
      /// Gets the differences.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="compareList">The second list.</param>
      /// <returns>System.ValueTuple&lt;IList&lt;T&gt;, IList&lt;T&gt;&gt;.</returns>
      /// <return>
      /// The differences between the two lists:
      /// * First tuple -- the main list items not in the second list
      /// * Second tuple -- the second list items not fond in the main list
      /// </return>
      public static (IList<T>, IList<T>) GetDifferences<T>( this T[] mainList, T[] compareList )
      {
         var mainListItemsNotFoundInSecondList = CreateDiffFromList( mainList,    compareList );
         var secondListItemsNotFoundInMainList = CreateDiffFromList( compareList, mainList );

         return ( mainListItemsNotFoundInSecondList, secondListItemsNotFoundInMainList );

         // PRIVATE METHODS
         List<T> CreateDiffFromList( T[] firstList, T[] secondList )
         {
            var retList = new List<T>();
            if ( firstList.IsNotAnEmptyList() )
            {
               foreach ( var firstListItem in firstList )
               {
                  if ( secondList.IsAnEmptyList() || !secondList.Contains( firstListItem ) )
                  {
                     retList.Add( firstListItem );
                  }
               }
            }

            return retList;
         }
      }

      /// <summary>
      /// Gets the enum count.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns>System.Int32.</returns>
      /// <exception cref="System.Exception">Not enum</exception>
      /// <exception cref="Exception">Not enum</exception>
      public static int GetEnumCount<T>()
      {
         if ( !typeof( T ).IsEnum )
         {
            throw new Exception( "Not enum" );
         }

         return Enum.GetNames( typeof( T ) ).Length;
      }

      /// <summary>
      /// Gets the properties for a type that have a public setter.  Digs through derived hierarchies.
      /// </summary>
      /// <param name="type">The type being analyzed.</param>
      /// <returns>The public settable property info's for this type.</returns>
      /// <remarks>This method is NOT THREAD SAFE due to the list add.</remarks>
      public static PropertyInfo[] GetRuntimeWriteableProperties( this Type type )
      {
         if ( !type.IsInterface )
         {
            return type.GetShallowRuntimeWriteableProperties();
         }

         // ELSE
         var propertyInfos = new List<PropertyInfo>();

         var considered = new List<Type>();
         var queue      = new Queue<Type>();
         considered.Add( type );
         queue.Enqueue( type );
         while ( queue.Count > 0 )
         {
            var subType = queue.Dequeue();
            foreach ( var subInterface in subType.GetInterfaces() )
            {
               if ( considered.Contains( subInterface ) )
               {
                  continue;
               }

               considered.Add( subInterface );
               queue.Enqueue( subInterface );
            }

            var typeProperties = subType.GetShallowRuntimeWriteableProperties();

            var newPropertyInfos = typeProperties
              .Where( x => !propertyInfos.Contains( x ) );

            propertyInfos.InsertRange( 0, newPropertyInfos );
         }

         return propertyInfos.ToArray();
      }

      /// <summary>
      /// Gets the properties for a single type only (no digging through nested elements)
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      public static PropertyInfo[] GetShallowRuntimeWriteableProperties( this Type type )
      {
         return type.GetProperties( PROP_FLAGS )
                    .Where( p => p.GetSetMethod().IsNotNullOrDefault() && p.GetGetMethod().IsNotNullOrDefault() )
                    .ToArray();
      }

      /// <summary>
      /// Gets the string from object.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <returns>System.String.</returns>
      public static string GetStringFromObject( object value )
      {
         if ( value is string s )
         {
            return s.IsEmpty<char>() ? string.Empty : s;
         }

         return value == null ? string.Empty : value.ToString();
      }

      /// <summary>
      /// Determines whether [has no value] [the specified database].
      /// </summary>
      /// <param name="db">The database.</param>
      /// <returns><c>true</c> if [has no value] [the specified database]; otherwise, <c>false</c>.</returns>
      public static bool HasNoValue( this double? db )
      {
         return !db.HasValue;
      }

      /// <summary>
      /// Determines whether [is an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEmptyList( this IList list )
      {
         return list.IsNullOrDefault() || ( list.Count == 0 );
      }

      /// <summary>
      /// Determines whether [is an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEmptyList( this IEnumerable list )
      {
         if ( list != null )
         {
            foreach ( var unused in list )
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      /// Determines whether [is an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualObjectTo
      (
         this object mainObj,
         object      compareObj
      )
      {
         return ( ( mainObj == null ) && ( compareObj == null ) )
              ||
                ( ( mainObj != null ) && mainObj.Equals( compareObj ) )
              ||
                ( ( compareObj != null ) && compareObj.Equals( mainObj ) );
      }

      /// <summary>
      /// Determines whether [is an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualReferenceTo<T>
      (
         this T mainObj,
         T      compareObj
      )
         where T : class
      {
         return ( ( mainObj == null ) && ( compareObj == null ) )
              ||
                ( ( mainObj == null == ( compareObj == null ) )
               && ReferenceEquals( compareObj, mainObj ) );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is different than] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this DateTime mainDateTime,
         DateTime      otherDateTime
      )
      {
         return !mainDateTime.IsSameAs( otherDateTime );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is different than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this double? mainD,
         double?      otherD
      )
      {
         return !mainD.IsSameAs( otherD );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is different than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this double mainD,
         double      otherD
      )
      {
         return !mainD.IsSameAs( otherD );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <returns><c>true</c> if [is different than] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this float mainF,
         float      otherF
      )
      {
         return !mainF.IsSameAs( otherF );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <returns><c>true</c> if [is different than] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this string mainStr,
         string      otherStr
      )
      {
         return !mainStr.IsSameAs( otherStr );
      }

      /// <summary>
      /// Determines whether [is different than] [the specified second list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="secondList">The second list.</param>
      /// <returns><c>true</c> if [is different than] [the specified second list]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan<T>( this T[] mainList, T[] secondList )
      {
         return !mainList.IsSameAs( secondList );
      }

      /// <summary>
      /// Determines whether [is not the same] [the specified second].
      /// </summary>
      /// <param name="first">if set to <c>true</c> [first].</param>
      /// <param name="second">if set to <c>true</c> [second].</param>
      /// <returns><c>true</c> if [is not the same] [the specified second]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this bool? first,
         bool?      second
      )
      {
         return ( ( first == null ) != ( second == null ) )
              ||
                first.IsNotAnEqualObjectTo( second );
      }

      /// <summary>
      /// Determines whether the specified main date time is empty.
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if the specified main date time is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty( this DateTime mainDateTime )
      {
         return mainDateTime.IsSameAs( default );
      }

      /// <summary>
      /// Determines whether the specified list is empty.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if the specified list is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty<T>( this IEnumerable<T> list )
      {
         return ( list == null ) || !list.Any();
      }

      /// <summary>
      /// Determines whether the specified main d is empty.
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if the specified main d is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty( this double mainD )
      {
         return mainD.IsSameAs( 0 );
      }

      /// <summary>
      /// Determines whether the specified string is empty.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if the specified string is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty( this string str )
      {
         return string.IsNullOrWhiteSpace( str );
      }

      /// <summary>
      /// Determines whether the specified b is false.
      /// </summary>
      /// <param name="b">if set to <c>true</c> [b].</param>
      /// <returns><c>true</c> if the specified b is false; otherwise, <c>false</c>.</returns>
      public static bool IsFalse( this bool? b )
      {
         return b.HasValue && !b.Value;
      }

      /// <summary>
      /// Determines whether [is greater than] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is greater than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsGreaterThan
      (
         this double thisD,
         double      otherD,
         double      numericError = NUMERIC_ERROR
      )
      {
         return ( thisD - otherD ) > numericError;
      }

      /// <summary>
      /// Determines whether [is greater than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is greater than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsGreaterThanOrEqualTo
      (
         this double thisD,
         double      otherD,
         double      numericError = NUMERIC_ERROR
      )
      {
         return thisD.IsSameAs( otherD, numericError ) || thisD.IsGreaterThan( otherD, numericError );
      }

      /// <summary>
      /// Determines whether [is less than] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is less than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsLessThan
      (
         this double thisD,
         double      otherD,
         double      numericError = NUMERIC_ERROR
      )
      {
         return ( otherD - thisD ) > numericError;
      }

      /// <summary>
      /// Determines whether [is less than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is less than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsLessThanOrEqualTo
      (
         this double thisD,
         double      otherD,
         double      numericError = NUMERIC_ERROR
      )
      {
         return thisD.IsSameAs( otherD, numericError ) || thisD.IsLessThan( otherD, numericError );
      }

      /// <summary>
      /// Determines whether [is non null regex match] [the specified regex].
      /// </summary>
      /// <param name="s">The s.</param>
      /// <param name="regex">The regex.</param>
      /// <returns><c>true</c> if [is non null regex match] [the specified regex]; otherwise, <c>false</c>.</returns>
      public static bool IsNonNullRegexMatch
      (
         this string s,
         string      regex
      )
      {
         return ( s != null ) && Regex.IsMatch( s, regex, RegexOptions.IgnoreCase );
      }

      /// <summary>
      /// Determines whether [is not an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEmptyList( this IList list )
      {
         return !list.IsAnEmptyList();
      }

      /// <summary>
      /// Determines whether [is not an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEmptyList( this IEnumerable list )
      {
         return !list.IsAnEmptyList();
      }

      /// <summary>
      /// Determines whether [is not an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualObjectTo
      (
         this object mainObj,
         object      compareObj
      )
      {
         return !mainObj.IsAnEqualObjectTo( compareObj );
      }

      /// <summary>
      /// Determines whether [is not an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualReferenceTo<T>
      (
         this T mainObj,
         T      compareObj
      )
         where T : class
      {
         return !mainObj.IsAnEqualReferenceTo( compareObj );
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified main date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main date time]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty( this DateTime mainDateTime )
      {
         return !mainDateTime.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not empty] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty<T>( this IEnumerable<T> list )
      {
         var answer = !list.IsEmpty();
         return answer;
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified main d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main d]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty( this double mainD )
      {
         return !mainD.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified string].
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if [is not empty] [the specified string]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty( this string str )
      {
         return !str.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not null or default] [the specified main object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <returns><c>true</c> if [is not null or default] [the specified main object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotNullOrDefault<T>( this T mainObj )
         where T : class
      {
         return !mainObj.IsNullOrDefault();
      }

      /// <summary>
      /// Determines whether [is null or default] [the specified main object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <returns><c>true</c> if [is null or default] [the specified main object]; otherwise, <c>false</c>.</returns>
      public static bool IsNullOrDefault<T>( this T mainObj )
         where T : class
      {
         return mainObj is null || mainObj.IsAnEqualReferenceTo( default );
      }

      /// <summary>
      /// Determines whether [is null or default] [the specified main object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <returns><c>true</c> if [is null or default] [the specified main object]; otherwise, <c>false</c>.</returns>
      public static bool IsNullOrDefault( this object mainObj )
      {
         return mainObj is null;
      }

      /// <summary>
      /// Determines whether [is really null] [the specified text].
      /// </summary>
      /// <param name="text">The text.</param>
      /// <returns><c>true</c> if [is really null] [the specified text]; otherwise, <c>false</c>.</returns>
      public static bool IsReallyNull( this string text )
      {
         return ( text == "" ) || ( text == string.Empty ) || ( text == null );
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is same as] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this double? mainD,
         double?      otherD
      )
      {
         return
            ( !mainD.HasValue && !otherD.HasValue )
          ||
            ( mainD.HasValue && otherD.HasValue && mainD.GetValueOrDefault().IsSameAs( otherD.GetValueOrDefault() ) );
      }

      /// <summary>
      /// Determines whether [is same as] [the specified second list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="secondList">The second list.</param>
      /// <returns><c>true</c> if [is same as] [the specified second list]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs<T>( this T[] mainList, T[] secondList )
      {
         var comparison = mainList.GetDifferences( secondList );

         return comparison.Item1.IsEmpty() && comparison.Item2.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is same as] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this DateTime mainDateTime,
         DateTime      otherDateTime
      )
      {
         return mainDateTime.CompareTo( otherDateTime ) == 0;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is same as] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this double mainD,
         double      otherD,
         double      numericError = NUMERIC_ERROR
      )
      {
         return Math.Abs( mainD - otherD ) < numericError;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is same as] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this float mainF,
         float      otherF,
         double     numericError = NUMERIC_ERROR
      )
      {
         return Math.Abs( mainF - otherF ) < numericError;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <param name="compareType">Type of the compare.</param>
      /// <returns><c>true</c> if [is same as] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this string      mainStr,
         string           otherStr,
         StringComparison compareType = StringComparison.CurrentCultureIgnoreCase
      )
      {
         var mainStrIsNullOrEmpty  = string.IsNullOrEmpty( mainStr );
         var otherStrIsNullOrEmpty = string.IsNullOrEmpty( otherStr );
         var isSameBasedOnNull     = mainStrIsNullOrEmpty && otherStrIsNullOrEmpty;

         if ( isSameBasedOnNull )
         {
            return true;
         }

         var isSameBasedOnComparison =
            string.Compare( mainStr, otherStr, compareType ) == 0;

         return isSameBasedOnComparison;
      }

      /// <summary>
      /// Determines whether the specified b is true.
      /// </summary>
      /// <param name="b">if set to <c>true</c> [b].</param>
      /// <returns><c>true</c> if the specified b is true; otherwise, <c>false</c>.</returns>
      public static bool IsTrue( this bool? b )
      {
         return b.HasValue && b.Value;
      }

      /// <summary>
      /// Determines whether [is type or assignable from type] [the specified target type].
      /// </summary>
      /// <param name="mainType">Type of the main.</param>
      /// <param name="targetType">Type of the target.</param>
      /// <returns><c>true</c> if [is type or assignable from type] [the specified target type]; otherwise, <c>false</c>.</returns>
      public static bool IsTypeOrAssignableFromType
      (
         this Type mainType,
         Type      targetType
      )
      {
         return ( mainType == targetType ) || targetType.IsAssignableFrom( mainType );
      }

      /// <summary>
      /// Minimums the of two doubles.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns>System.Double.</returns>
      public static double MinOfTwoDoubles
      (
         double width,
         double height
      )
      {
         return Math.Min( Convert.ToSingle( width ), Convert.ToSingle( height ) );
      }

      /// <summary>
      /// Positions the of decimal.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns>System.Int32.</returns>
      public static int PositionOfDecimal( this string str )
      {
         return str.IndexOf( DECIMAL, StringComparison.InvariantCultureIgnoreCase );
      }

      public static bool RandomBool( Random random = null )
      {
         // Random misses the last element, so 2 instead of 1
         var boolAsInt = ( random ?? CreateRandom ).Next( 0, 2 );

         return boolAsInt == 0;
      }

      /// <summary>
      /// </summary>
      /// <param name="date"></param>
      /// <param name="daysBefore">Always a positive number</param>
      /// <param name="daysAfter">Always a positive number</param>
      /// <param name="random"></param>
      /// <returns></returns>
      public static DateTime? RandomDateTimeFromHere( this DateTime? date, int daysBefore, int daysAfter,
                                                      Random         random = null )
      {
         if ( !date.HasValue || ( daysBefore < 0 ) || ( daysAfter < 0 ) )
         {
            return default;
         }

         var localRandom = random ?? CreateRandom;

         // Random misses the last element, so adding 1 to daysAfter
         var randomDays = localRandom.Next( -daysBefore, daysAfter + 1 );

         var randomHours   = localRandom.Next( 0, 25 );
         var randomMinutes = localRandom.Next( 0, 61 );
         var randomSeconds = localRandom.Next( 0, 61 );

         // Random offset; just adding to the randomDay.  It won't matter if tha is positive or negative.
         var randomTimeSpanToAdd = new TimeSpan( randomDays, randomHours, randomMinutes, randomSeconds, 0 );

         // Could be positive or negative
         return date.GetValueOrDefault().Add( randomTimeSpanToAdd );
      }

      public static DateTime? RandomDateTimeFromHere( this DateTime date, int daysBefore, int daysAfter,
                                                      Random        random = null )
      {
         return new DateTime?( date ).RandomDateTimeFromHere( daysBefore, daysAfter );
      }

      /// <summary>
      /// Randoms the string.
      /// </summary>
      /// <param name="strings">The strings.</param>
      /// <param name="emptyOK">if set to <c>true</c> [empty ok].</param>
      /// <param name="random"></param>
      /// <returns>System.String.</returns>
      public static string RandomString( this string[] strings, bool emptyOK = false, Random random = null )
      {
         if ( strings.IsEmpty() )
         {
            return "";
         }

         var validStrings = emptyOK ? strings : strings.Where( s => s.IsNotEmpty() ).ToArray();

         // Random never returns the last member, so adding 1 to "validStrings.Length"
         var nextRandomString = validStrings[ ( random ?? CreateRandom ).Next( 0, validStrings.Length ) ];

         return nextRandomString;
      }

      /// <summary>
      /// Reverses the dictionary.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <typeparam name="U"></typeparam>
      /// <param name="template">The template.</param>
      /// <returns>ConcurrentDictionary&lt;U, T&gt;.</returns>
      public static ConcurrentDictionary<U, T> ReverseDictionary<T, U>( ConcurrentDictionary<T, U> template )
      {
         // Declare the return dictionary backwards
         var retDict = new ConcurrentDictionary<U, T>();

         foreach ( var keyVauePair in template )
         {
            // Check the value and not the key
            if ( !retDict.ContainsKey( keyVauePair.Value ) )
            {
               // Add the values backwards retDict.AddOrUpdate(keyVauePair.Value, keyVauePair.Key, (k, v) => v);
               retDict.AddOrUpdate( keyVauePair.Value, keyVauePair.Key );
            }
         }

         return retDict;
      }

      /// <summary>
      /// Rounds to int.
      /// </summary>
      /// <param name="floatVal">The float value.</param>
      /// <returns>System.Int32.</returns>
      public static int RoundToInt( this double floatVal )
      {
         return (int)Math.Round( floatVal, 0 );
      }

      /// <summary>
      /// Converts to roundedint.
      /// </summary>
      /// <param name="d">The d.</param>
      /// <returns>System.Int32.</returns>
      public static int ToRoundedInt( this double d )
      {
         return (int)Math.Round( d, 0 );
      }

      /// <summary>
      /// Converts to roundedlong.
      /// </summary>
      /// <param name="d">The d.</param>
      /// <returns>System.Int64.</returns>
      public static long ToRoundedLong( this double d )
      {
         return (long)Math.Round( d, 0 );
      }

      /// <summary>
      /// Returns the value for a key, if that key exists in the dictionary.
      /// </summary>
      /// <param name="dict">The dictionary.</param>
      /// <param name="key">The key.</param>
      /// <returns>System.String.</returns>
      /// <remarks>This is *not* thread safe</remarks>
      public static string TryToGetKeyValue
      (
         this IDictionary<string, string> dict,
         string                           key
      )
      {
         if ( ( dict != null ) && dict.ContainsKey( key ) )
         {
            return dict[ key ];
         }

         return string.Empty;
      }
   }
}