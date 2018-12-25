// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="Extensions.cs" company="Marcus Technical Services, Inc.">
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

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;

   /// <summary>
   /// Class Extensions.
   /// </summary>
   public static class Extensions
   {
      #region Private Fields

      /// <summary>
      /// The numeric error
      /// </summary>
      private const double NUMERIC_ERROR = 0.001;

      #endregion Private Fields

      #region Public Properties

      /// <summary>
      /// Gets a value indicating whether [empty nullable bool].
      /// </summary>
      /// <value><c>null</c> if [empty nullable bool] contains no value, <c>true</c> if [empty nullable bool]; otherwise,
      /// <c>false</c>.</value>
      public static bool? EmptyNullableBool => new bool?();

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Determines if two collections of properties contain the same actual values. Can be called
      /// for a single property using the optional parameter.
      /// </summary>
      /// <typeparam name="T">The type of class which is being evaluated for changes.</typeparam>
      /// <param name="mainViewModel">The main class for comparison.</param>
      /// <param name="otherViewModel">The secondary class for comparison.</param>
      /// <param name="singlePropertyName">If set, this will be the only property interrogated.</param>
      /// <param name="propInfos">The property info collection that will be analyzed for changes.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool AnySettablePropertyHasChanged<T>(this T                mainViewModel,
                                                          T                     otherViewModel,
                                                          string                singlePropertyName = default(string),
                                                          params PropertyInfo[] propInfos)
      {
         var isChanged = false;

         foreach (var propInfo in propInfos)
         {
            // If we have a single property, only allow that one through.
            if (singlePropertyName.IsNotEmpty<char>() && propInfo.Name.IsDifferentThan(singlePropertyName))
            {
               continue;
            }

            var currentValue = propInfo.GetValue(mainViewModel);
            var otherValue   = propInfo.GetValue(otherViewModel);

            if (currentValue.IsNotAnEqualObjectTo(otherValue))
            {
               isChanged = true;
               break;
            }
         }

         return isChanged;
      }

      /// <summary>
      /// Cleans up service error text.
      /// </summary>
      /// <param name="errorText">The error text.</param>
      /// <returns>System.String.</returns>
      public static string CleanUpServiceErrorText(this string errorText)
      {
         // Find the LAST colon in the string
         var colonPos = errorText.LastIndexOf(":", StringComparison.CurrentCultureIgnoreCase);
         if (colonPos > 0)
         {
            var newErrorText = errorText.Substring(colonPos + 1);
            newErrorText = newErrorText.Trim('{');
            newErrorText = newErrorText.Trim('}');
            newErrorText = newErrorText.Trim('"');

            return newErrorText;
         }

         return string.Empty;
      }

      //#if AUDIT_PROP_INFO
      //            Debug.WriteLine("SUCCESS: copied prop info ->" + propInfo.Name + "<-");
      //#endif
      //         }
      //       }
      //       catch (Exception ex)
      //       {
      //         Debug.WriteLine("CopySettablePropertyValuesFrom error ->" + ex.Message + "<-");
      //       }
      //     }
      /// <summary>
      /// Gets all property infos.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns>PropertyInfo[].</returns>
      public static PropertyInfo[] GetAllPropInfos<T>()
      {
         var retInfos = typeof(T).GetRuntimeWriteableProperties();
         return retInfos.ToArray();
      }

      /// <summary>
      /// Gets the enum count.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns>System.Int32.</returns>
      /// <exception cref="Exception">Not enum</exception>
      /// <exception cref="System.Exception">Not enum</exception>
      public static int GetEnumCount<T>()
      {
         if (!typeof(T).IsEnum)
         {
            throw new Exception("Not enum");
         }

         return Enum.GetNames(typeof(T)).Length;
      }

      /// <summary>
      /// Gets the properties for a type that have a public setter.
      /// </summary>
      /// <param name="type">The type being analyzed.</param>
      /// <returns>The public settable property info's for this type.</returns>
      /// <remarks>This method is NOT THREAD SAFE due to the list add.</remarks>
      public static PropertyInfo[] GetRuntimeWriteableProperties(this Type type)
      {
         if (!type.IsInterface)
         {
            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
         }

         // ELSE
         var propertyInfos = new List<PropertyInfo>();

         var considered = new List<Type>();
         var queue      = new Queue<Type>();
         considered.Add(type);
         queue.Enqueue(type);
         while (queue.Count > 0)
         {
            var subType = queue.Dequeue();
            foreach (var subInterface in subType.GetInterfaces())
            {
               if (considered.Contains(subInterface))
               {
                  continue;
               }

               considered.Add(subInterface);
               queue.Enqueue(subInterface);
            }

            var typeProperties = subType.GetProperties(
                                                       BindingFlags.FlattenHierarchy
                                                     | BindingFlags.Public
                                                     | BindingFlags.Instance);

            var newPropertyInfos = typeProperties
              .Where(x => !propertyInfos.Contains(x));

            propertyInfos.InsertRange(0, newPropertyInfos);
         }

         return propertyInfos.ToArray();
      }

      /// <summary>
      /// Gets the string from object.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <returns>System.String.</returns>
      public static string GetStringFromObject(object value)
      {
         if (value is string s)
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
      public static bool HasNoValue(this double? db)
      {
         return !db.HasValue;
      }

      /// <summary>
      /// Determines whether [is an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualObjectTo(this object mainObj,
                                           object      compareObj)
      {
         return mainObj == null && compareObj == null
              ||
                mainObj != null && mainObj.Equals(compareObj)
              ||
                compareObj != null && compareObj.Equals(mainObj);
      }

      /// <summary>
      /// Determines whether [is an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualReferenceTo<T>(this T mainObj,
                                                 T      compareObj)
         where T : class
      {
         return mainObj == null && compareObj == null
              ||
                mainObj == null == (compareObj == null)
             && ReferenceEquals(compareObj, mainObj);
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is different than] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan(this DateTime mainDateTime,
                                         DateTime      otherDateTime)
      {
         return !mainDateTime.IsSameAs(otherDateTime);
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is different than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan(this double mainD,
                                         double      otherD)
      {
         return !mainD.IsSameAs(otherD);
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <returns><c>true</c> if [is different than] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan(this float mainF,
                                         float      otherF)
      {
         return !mainF.IsSameAs(otherF);
      }

      /// <summary>
      /// Determines whether [is different than] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <returns><c>true</c> if [is different than] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan(this string mainStr,
                                         string      otherStr)
      {
         return !mainStr.IsSameAs(otherStr);
      }

      /// <summary>
      /// Determines whether the specified main date time is empty.
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if the specified main date time is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this DateTime mainDateTime)
      {
         return mainDateTime.IsSameAs(default(DateTime));
      }

      /// <summary>
      /// Determines whether the specified list is empty.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if the specified list is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty<T>(this IEnumerable<T> list)
      {
         return list == null || !list.Any();
      }

      /// <summary>
      /// Determines whether the specified main d is empty.
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if the specified main d is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this double mainD)
      {
         return mainD.IsSameAs(0);
      }

      /// <summary>
      /// Determines whether the specified string is empty.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if the specified string is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this string str)
      {
         return string.IsNullOrWhiteSpace(str);
      }

      /// <summary>
      /// Determines whether [is greater than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is greater than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsGreaterThanOrEqualTo(this double thisD,
                                                double      otherD)
      {
         return thisD.IsSameAs(otherD) || thisD > otherD;
      }

      /// <summary>
      /// Determines whether [is less than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is less than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsLessThanOrEqualTo(this double thisD,
                                             double      otherD)
      {
         return thisD.IsSameAs(otherD) || thisD < otherD;
      }

      /// <summary>
      /// Determines whether [is not an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualObjectTo(this object mainObj,
                                              object      compareObj)
      {
         return !mainObj.IsAnEqualObjectTo(compareObj);
      }

      /// <summary>
      /// Determines whether [is not an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualReferenceTo<T>(this T mainObj,
                                                    T      compareObj)
         where T : class
      {
         return !mainObj.IsAnEqualReferenceTo(compareObj);
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified main date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main date time]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this DateTime mainDateTime)
      {
         return !mainDateTime.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not empty] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty<T>(this IEnumerable<T> list)
      {
         return !list.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified main d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main d]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this double mainD)
      {
         return !mainD.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not empty] [the specified string].
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if [is not empty] [the specified string]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this string str)
      {
         return !str.IsEmpty();
      }

      /// <summary>
      /// Determines whether [is not the same] [the specified second].
      /// </summary>
      /// <param name="first">if set to <c>true</c> [first].</param>
      /// <param name="second">if set to <c>true</c> [second].</param>
      /// <returns><c>true</c> if [is not the same] [the specified second]; otherwise, <c>false</c>.</returns>
      public static bool IsNotTheSame(this bool? first,
                                      bool?      second)
      {
         return first == null != (second == null)
              ||
                first.IsNotAnEqualObjectTo(second);
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is same as] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs(this DateTime mainDateTime,
                                  DateTime      otherDateTime)
      {
         return mainDateTime.CompareTo(otherDateTime) == 0;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is same as] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs(this double mainD,
                                  double      otherD)
      {
         return Math.Abs(mainD - otherD) < NUMERIC_ERROR;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <returns><c>true</c> if [is same as] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs(this float mainF,
                                  float      otherF)
      {
         return Math.Abs(mainF - otherF) < NUMERIC_ERROR;
      }

      /// <summary>
      /// Determines whether [is same as] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <returns><c>true</c> if [is same as] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs(this string mainStr,
                                  string      otherStr)
      {
         return string.Compare(mainStr, otherStr, StringComparison.CurrentCultureIgnoreCase) == 0;
      }

      /// <summary>
      /// Determines whether the specified b is true.
      /// </summary>
      /// <param name="b">if set to <c>true</c> [b].</param>
      /// <returns><c>true</c> if the specified b is true; otherwise, <c>false</c>.</returns>
      public static bool IsTrue(this bool? b)
      {
         return b.HasValue && b.Value;
      }

      /// <summary>
      /// Determines whether [is type or assignable from type] [the specified target type].
      /// </summary>
      /// <param name="mainType">Type of the main.</param>
      /// <param name="targetType">Type of the target.</param>
      /// <returns><c>true</c> if [is type or assignable from type] [the specified target type]; otherwise, <c>false</c>.</returns>
      public static bool IsTypeOrAssignableFromType(this Type mainType,
                                                    Type      targetType)
      {
         return mainType == targetType || targetType.IsAssignableFrom(mainType);
      }

      /// <summary>
      /// Minimums the of two doubles.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns>System.Double.</returns>
      public static double MinOfTwoDoubles(double width,
                                           double height)
      {
         return Math.Min(Convert.ToSingle(width), Convert.ToSingle(height));
      }

      /// <summary>
      /// Rounds to int.
      /// </summary>
      /// <param name="floatVal">The float value.</param>
      /// <returns>System.Int32.</returns>
      public static int RoundToInt(this double floatVal)
      {
         return (int) Math.Round(floatVal, 0);
      }

      /// <summary>
      /// Converts to roundedint.
      /// </summary>
      /// <param name="d">The d.</param>
      /// <returns>System.Int32.</returns>
      public static int ToRoundedInt(this double d)
      {
         return (int) Math.Round(d, 0);
      }

      /// <summary>
      /// Returns the value for a key, if that key exists in the dictionary.
      /// </summary>
      /// <param name="dict">The dictionary.</param>
      /// <param name="key">The key.</param>
      /// <returns>System.String.</returns>
      /// <remarks>This is *not* thread safe</remarks>
      public static string TryToGetKeyValue(this IDictionary<string, string> dict,
                                            string                           key)
      {
         if (dict != null && dict.ContainsKey(key))
         {
            return dict[key];
         }

         return string.Empty;
      }

      #endregion Public Methods
   }
}