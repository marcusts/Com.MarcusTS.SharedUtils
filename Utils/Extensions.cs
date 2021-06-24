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
   ///    Class Extensions.
   /// </summary>
   public static class Extensions
   {
      /// <summary>
      ///    The decimal
      /// </summary>
      public const string DECIMAL = ".";

      /// <summary>
      ///    The zero character
      /// </summary>
      public const char ZERO_CHAR = '0';

      /// <summary>
      ///    The global random
      /// </summary>
      public static readonly Random GLOBAL_RANDOM = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

      /// <summary>
      ///    The numeric error
      /// </summary>
      private const double NUMERIC_ERROR = 0.001;

      /// <summary>
      ///    Gets a value indicating whether [empty nullable bool].
      /// </summary>
      /// <value>
      ///    <c>null</c> if [empty nullable bool] contains no value, <c>true</c> if [empty nullable bool]; otherwise,
      ///    <c>false</c>.
      /// </value>
      public static bool? EmptyNullableBool => new bool?();

      /// <summary>
      ///    Adds the or update.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <typeparam name="U"></typeparam>
      /// <param name="retDict">The ret dictionary.</param>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public static void AddOrUpdate<T, U>
      (
         this ConcurrentDictionary<T, U> retDict,
         T key,
         U value
      )
      {
         retDict.AddOrUpdate(key, value,
                             (
                                k,
                                v
                             ) => v);
      }

      /// <summary>
      ///    Determines if two collections of properties contain the same actual values. Can be called
      ///    for a single property using the optional parameter.
      /// </summary>
      /// <typeparam name="T">The type of class which is being evaluated for changes.</typeparam>
      /// <param name="mainViewModel">The main class for comparison.</param>
      /// <param name="otherViewModel">The secondary class for comparison.</param>
      /// <param name="singlePropertyName">If set, this will be the only property interrogated.</param>
      /// <param name="propInfos">The property info collection that will be analyzed for changes.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool AnySettablePropertyHasChanged<T>
      (
         this T mainViewModel,
         T otherViewModel,
         string singlePropertyName,
         PropertyInfo[] propInfos
      )
         where T : class
      {
         if (mainViewModel.IsNullOrDefault())
         {
            return false;
         }

         var isChanged = false;

         foreach (var propInfo in propInfos)
         {
            // If we have a single property, only allow that one through.
            if (singlePropertyName.IsNotEmpty() && propInfo.Name.IsDifferentThan(singlePropertyName))
            {
               continue;
            }

            var currentValue = propInfo.GetValue(mainViewModel);
            var otherValue = propInfo.GetValue(otherViewModel);

            if (currentValue.IsNotAnEqualObjectTo(otherValue))
            {
               isChanged = true;
               break;
            }
         }

         return isChanged;
      }

      /// <summary>
      ///    Cleans up service error text.
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

      public static OutputListInterface[] CopyEnumerable<InputListInterface, InputListClass, OutputListInterface, OutputListClass>(InputListClass[] inputList, Func<OutputListClass> creator)
         where InputListClass : class, InputListInterface
         where OutputListClass : class, OutputListInterface, InputListInterface
      {
         if (inputList.IsAnEmptyList())
         {
            return default;
         }

         var propInfos = typeof(InputListClass).GetRuntimeWriteableProperties().ToArray();

         if (propInfos.IsAnEmptyList())
         {
            return default;
         }

         var tempOutputList = new List<OutputListInterface>();

         // The properties should be of the individual list type - not the originating service model type.
         foreach (var record in inputList)
         {
            var newRecord = creator?.Invoke();
            newRecord.CopySettablePropertyValuesFrom<InputListInterface>(record, propInfos);
            tempOutputList.Add(newRecord);
         }

         return tempOutputList.ToArray();
      }

      /// <summary>
      ///    Copy the values from the specified properties from value to target.
      /// </summary>
      /// <typeparam name="T">*Unused* -- required for referencing only.</typeparam>
      /// <param name="targetViewModel">The view model to copy *to*.</param>
      /// <param name="valueViewModel">The view model to copy *from*.</param>
      /// <param name="propInfos">The property info records to use to get and set values.</param>
      public static void CopySettablePropertyValuesFrom<T>
      (
         this T targetViewModel,
         T valueViewModel,
         PropertyInfo[] propInfos = null
      )
      {
         if (propInfos == null || propInfos.IsAnEmptyList())
         {
            propInfos = typeof(T).GetRuntimeWriteableProperties();
         }

         try
         {
            foreach (var propInfo in propInfos)
            {
               propInfo.SetValue(targetViewModel, propInfo.GetValue(valueViewModel));
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine("CopySettablePropertyValuesFrom error ->" + ex.Message + "<-");
         }
      }

      /// <summary>
      /// Coerces property values between two interfaces using rules.
      /// </summary>
      /// <param name="targetViewModel">The target view model ("this")</param>
      /// <param name="sourceViewModel">The source view model.</param>
      /// <param name="coerceProperty">Changes the propety name (as needed) so it can be found in the targetViewModel</param>
      /// <param name="targetPropInfos">The property info collection for the target view model.</param>
      /// <param name="sourcePropInfos">The property info collection for the source view model.</param>
      public static void CoerceSettablePropertyValuesFrom<TargetT, SourceT>
      (
         this TargetT                                     targetViewModel,
         SourceT                                           sourceViewModel,
         Func<PropertyInfo, SourceT, (bool, string, object)> coerceProperty,
         PropertyInfo[]                                   targetPropInfos = default,
         PropertyInfo[]                                   sourcePropInfos  = default
      )
      {
         if (sourcePropInfos == null || sourcePropInfos.IsAnEmptyList())
         {
            sourcePropInfos = typeof(SourceT).GetRuntimeWriteableProperties();
         }

         if (targetPropInfos == null || targetPropInfos.IsAnEmptyList())
         {
            targetPropInfos = typeof(TargetT).GetRuntimeWriteableProperties();
         }

         try
         {
            foreach (var valuePropInfo in sourcePropInfos)
            {
               var propNameAndValue = coerceProperty(valuePropInfo, sourceViewModel);
               
               // Make sure the property was found and we can proceed
               if (propNameAndValue.Item1)
               {
                  var foundPropInfo =
                     targetPropInfos.FirstOrDefault(pi => pi.Name.IsSameAs(propNameAndValue.Item2));

                  if (foundPropInfo.IsNotNullOrDefault())
                  {
                     // Assign the value
                     // ReSharper disable once PossibleNullReferenceException
                     foundPropInfo.SetValue(targetViewModel, propNameAndValue.Item3);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine(nameof(CoerceSettablePropertyValuesFrom) + " ERROR ->" + ex.Message + "<-");
         }
      }

      /// <summary>
      ///    Gets all property infos.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns>PropertyInfo[].</returns>
      public static PropertyInfo[] GetAllPropInfos<T>()
      {
         var retInfos = typeof(T).GetRuntimeWriteableProperties();
         return retInfos.ToArray();
      }

      /// <summary>
      ///    Gets the differences.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="secondList">The second list.</param>
      /// <returns>System.ValueTuple&lt;IList&lt;T&gt;, IList&lt;T&gt;&gt;.</returns>
      /// <return>
      ///    The differences between the two lists:
      ///    * First tuple -- the main list items not in the second list
      ///    * Second tuple -- the second list items not fond in the main list
      /// </return>
      public static (IList<T>, IList<T>) GetDifferences<T>(this T[] mainList, T[] secondList)
      {
         var mainListItemsNotFoundInSecondList = new List<T>();
         var secondListItemsNotFoundInMainList = new List<T>();

         if (mainList.IsNotAnEmptyList())
         {
            foreach (var mainListItem in mainList)
            {
               if (secondList.IsAnEmptyList() || !secondList.Contains(mainListItem))
               {
                  mainListItemsNotFoundInSecondList.Add(mainListItem);
               }
            }
         }

         if (secondList.IsNotAnEmptyList())
         {
            foreach (var secondListItem in secondList)
            {
               if (mainList.IsAnEmptyList() || !mainList.Contains(secondListItem))
               {
                  secondListItemsNotFoundInMainList.Add(secondListItem);
               }
            }
         }

         return (mainListItemsNotFoundInSecondList, secondListItemsNotFoundInMainList);
      }

      /// <summary>
      ///    Gets the enum count.
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
      ///    Gets the properties for a type that have a public setter.
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
         var queue = new Queue<Type>();
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
      ///    Gets the string from object.
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
      ///    Determines whether [has no value] [the specified database].
      /// </summary>
      /// <param name="db">The database.</param>
      /// <returns><c>true</c> if [has no value] [the specified database]; otherwise, <c>false</c>.</returns>
      public static bool HasNoValue(this double? db)
      {
         return !db.HasValue;
      }

      /// <summary>
      ///    Determines whether [is an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEmptyList(this IList list)
      {
         return list.IsNullOrDefault() || list.Count == 0;
      }

      /// <summary>
      ///    Determines whether [is an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEmptyList(this IEnumerable list)
      {
         if (list != null)
         {
            foreach (var unused in list)
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///    Determines whether [is an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualObjectTo
      (
         this object mainObj,
         object compareObj
      )
      {
         return mainObj == null && compareObj == null
              ||
                mainObj != null && mainObj.Equals(compareObj)
              ||
                compareObj != null && compareObj.Equals(mainObj);
      }

      /// <summary>
      ///    Determines whether [is an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsAnEqualReferenceTo<T>
      (
         this T mainObj,
         T compareObj
      )
         where T : class
      {
         return mainObj == null && compareObj == null
              ||
                mainObj == null == (compareObj == null)
             && ReferenceEquals(compareObj, mainObj);
      }

      /// <summary>
      /// Determines whether [is a number greater than zero] [the specified o].
      /// </summary>
      /// <param name="o">The o.</param>
      /// <returns><c>true</c> if [is a number greater than zero] [the specified o]; otherwise, <c>false</c>.</returns>
      public static bool IsANumberGreaterThanZero(this object o)
      {
         if (double.TryParse(o?.ToString(), out var testDouble))
         {
            return testDouble.IsGreaterThan(0);
         }

         return false;
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is different than] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this DateTime mainDateTime,
         DateTime otherDateTime
      )
      {
         return !mainDateTime.IsSameAs(otherDateTime);
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is different than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this double? mainD,
         double? otherD
      )
      {
         return !mainD.IsSameAs(otherD);
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is different than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this double mainD,
         double otherD
      )
      {
         return !mainD.IsSameAs(otherD);
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <returns><c>true</c> if [is different than] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this float mainF,
         float otherF
      )
      {
         return !mainF.IsSameAs(otherF);
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <returns><c>true</c> if [is different than] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this string mainStr,
         string otherStr
      )
      {
         return !mainStr.IsSameAs(otherStr);
      }

      /// <summary>
      ///    Determines whether [is different than] [the specified second list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="secondList">The second list.</param>
      /// <returns><c>true</c> if [is different than] [the specified second list]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan<T>(this T[] mainList, T[] secondList)
      {
         return !mainList.IsSameAs(secondList);
      }

      /// <summary>
      ///    Determines whether [is not the same] [the specified second].
      /// </summary>
      /// <param name="first">if set to <c>true</c> [first].</param>
      /// <param name="second">if set to <c>true</c> [second].</param>
      /// <returns><c>true</c> if [is not the same] [the specified second]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this bool? first,
         bool? second
      )
      {
         return first == null != (second == null)
              ||
                first.IsNotAnEqualObjectTo(second);
      }

      /// <summary>
      ///    Determines whether the specified main date time is empty.
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if the specified main date time is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this DateTime mainDateTime)
      {
         return mainDateTime.IsSameAs(default);
      }

      /// <summary>
      ///    Determines whether the specified list is empty.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if the specified list is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty<T>(this IEnumerable<T> list)
      {
         return list == null || !list.Any();
      }

      /// <summary>
      ///    Determines whether the specified main d is empty.
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if the specified main d is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this double mainD)
      {
         return mainD.IsSameAs(0);
      }

      /// <summary>
      ///    Determines whether the specified string is empty.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if the specified string is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty(this string str)
      {
         return string.IsNullOrWhiteSpace(str);
      }

      /// <summary>
      ///    Determines whether [is greater than] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is greater than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsGreaterThan
      (
         this double thisD,
         double otherD,
         double numericError = NUMERIC_ERROR
      )
      {
         return thisD - otherD > numericError;
      }

      /// <summary>
      ///    Determines whether [is greater than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is greater than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsGreaterThanOrEqualTo
      (
         this double thisD,
         double otherD,
         double numericError = NUMERIC_ERROR
      )
      {
         return thisD.IsSameAs(otherD, numericError) || thisD.IsGreaterThan(otherD, numericError);
      }

      /// <summary>
      ///    Determines whether [is less than] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is less than] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsLessThan
      (
         this double thisD,
         double otherD,
         double numericError = NUMERIC_ERROR
      )
      {
         return otherD - thisD > numericError;
      }

      /// <summary>
      ///    Determines whether [is less than or equal to] [the specified other d].
      /// </summary>
      /// <param name="thisD">The this d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is less than or equal to] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsLessThanOrEqualTo
      (
         this double thisD,
         double otherD,
         double numericError = NUMERIC_ERROR
      )
      {
         return thisD.IsSameAs(otherD, numericError) || thisD.IsLessThan(otherD, numericError);
      }

      /// <summary>
      ///    Determines whether [is non null regex match] [the specified regex].
      /// </summary>
      /// <param name="s">The s.</param>
      /// <param name="regex">The regex.</param>
      /// <returns><c>true</c> if [is non null regex match] [the specified regex]; otherwise, <c>false</c>.</returns>
      public static bool IsNonNullRegexMatch
      (
         this string s,
         string regex
      )
      {
         return s != null && Regex.IsMatch(s, regex, RegexOptions.IgnoreCase);
      }

      /// <summary>
      ///    Determines whether [is not an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEmptyList(this IList list)
      {
         return !list.IsAnEmptyList();
      }

      /// <summary>
      ///    Determines whether [is not an empty list] [the specified list].
      /// </summary>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not an empty list] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEmptyList(this IEnumerable list)
      {
         return !list.IsAnEmptyList();
      }

      /// <summary>
      ///    Determines whether [is not an equal object to] [the specified compare object].
      /// </summary>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal object to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualObjectTo
      (
         this object mainObj,
         object compareObj
      )
      {
         return !mainObj.IsAnEqualObjectTo(compareObj);
      }

      /// <summary>
      ///    Determines whether [is not an equal reference to] [the specified compare object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <param name="compareObj">The compare object.</param>
      /// <returns><c>true</c> if [is not an equal reference to] [the specified compare object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotAnEqualReferenceTo<T>
      (
         this T mainObj,
         T compareObj
      )
         where T : class
      {
         return !mainObj.IsAnEqualReferenceTo(compareObj);
      }

      /// <summary>
      ///    Determines whether [is not empty] [the specified main date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main date time]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this DateTime mainDateTime)
      {
         return !mainDateTime.IsEmpty();
      }

      /// <summary>
      ///    Determines whether [is not empty] [the specified list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list">The list.</param>
      /// <returns><c>true</c> if [is not empty] [the specified list]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty<T>(this IEnumerable<T> list)
      {
         var answer = !list.IsEmpty();
         return answer;
      }

      /// <summary>
      ///    Determines whether [is not empty] [the specified main d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main d]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this double mainD)
      {
         return !mainD.IsEmpty();
      }

      /// <summary>
      ///    Determines whether [is not empty] [the specified string].
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns><c>true</c> if [is not empty] [the specified string]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty(this string str)
      {
         return !str.IsEmpty();
      }

      /// <summary>
      ///    Determines whether [is not null or default] [the specified main object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <returns><c>true</c> if [is not null or default] [the specified main object]; otherwise, <c>false</c>.</returns>
      public static bool IsNotNullOrDefault<T>(this T mainObj)
         where T : class
      {
         return !mainObj.IsNullOrDefault();
      }

      /// <summary>
      ///    Determines whether [is null or default] [the specified main object].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainObj">The main object.</param>
      /// <returns><c>true</c> if [is null or default] [the specified main object]; otherwise, <c>false</c>.</returns>
      public static bool IsNullOrDefault<T>(this T mainObj)
         where T : class
      {
         return mainObj == null || mainObj.IsAnEqualReferenceTo(default);
      }

      /// <summary>
      ///    Determines whether [is really null] [the specified text].
      /// </summary>
      /// <param name="text">The text.</param>
      /// <returns><c>true</c> if [is really null] [the specified text]; otherwise, <c>false</c>.</returns>
      public static bool IsReallyNull(this string text)
      {
         return text == "" || text == string.Empty || text == null;
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <returns><c>true</c> if [is same as] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this double? mainD,
         double? otherD
      )
      {
         return
            !mainD.HasValue && !otherD.HasValue
          ||
            mainD.HasValue && otherD.HasValue && mainD.GetValueOrDefault().IsSameAs(otherD.GetValueOrDefault());
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified second list].
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainList">The main list.</param>
      /// <param name="secondList">The second list.</param>
      /// <returns><c>true</c> if [is same as] [the specified second list]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs<T>(this T[] mainList, T[] secondList)
      {
         var comparison = mainList.GetDifferences(secondList);

         return comparison.Item1.IsEmpty() && comparison.Item2.IsEmpty();
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified other date time].
      /// </summary>
      /// <param name="mainDateTime">The main date time.</param>
      /// <param name="otherDateTime">The other date time.</param>
      /// <returns><c>true</c> if [is same as] [the specified other date time]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this DateTime mainDateTime,
         DateTime otherDateTime
      )
      {
         return mainDateTime.CompareTo(otherDateTime) == 0;
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified other d].
      /// </summary>
      /// <param name="mainD">The main d.</param>
      /// <param name="otherD">The other d.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is same as] [the specified other d]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this double mainD,
         double otherD,
         double numericError = NUMERIC_ERROR
      )
      {
         return Math.Abs(mainD - otherD) < numericError;
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified other f].
      /// </summary>
      /// <param name="mainF">The main f.</param>
      /// <param name="otherF">The other f.</param>
      /// <param name="numericError">The numeric error.</param>
      /// <returns><c>true</c> if [is same as] [the specified other f]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this float mainF,
         float otherF,
         double numericError = NUMERIC_ERROR
      )
      {
         return Math.Abs(mainF - otherF) < numericError;
      }

      /// <summary>
      ///    Determines whether [is same as] [the specified other string].
      /// </summary>
      /// <param name="mainStr">The main string.</param>
      /// <param name="otherStr">The other string.</param>
      /// <returns><c>true</c> if [is same as] [the specified other string]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this string mainStr,
         string otherStr
      )
      {
         var mainStrIsNullOrEmpty = string.IsNullOrEmpty(mainStr);
         var otherStrIsNullOrEmpty = string.IsNullOrEmpty(otherStr);
         var isSameBasedOnNull = mainStrIsNullOrEmpty && otherStrIsNullOrEmpty;

         if (isSameBasedOnNull)
         {
            return true;
         }

         var isSameBasedOnComparison =
            string.Compare(mainStr, otherStr, StringComparison.CurrentCultureIgnoreCase) == 0;

         //if (isSameBasedOnNull && !isSameBasedOnComparison)
         //{
         //   Debug.WriteLine(nameof(IsSameAs) + ": WarningException: Mismatched comparison");
         //}

         return isSameBasedOnComparison;
      }

      /// <summary>
      ///    Determines whether the specified b is true.
      /// </summary>
      /// <param name="b">if set to <c>true</c> [b].</param>
      /// <returns><c>true</c> if the specified b is true; otherwise, <c>false</c>.</returns>
      public static bool IsTrue(this bool? b)
      {
         return b.HasValue && b.Value;
      }

      /// <summary>
      ///    Determines whether [is type or assignable from type] [the specified target type].
      /// </summary>
      /// <param name="mainType">Type of the main.</param>
      /// <param name="targetType">Type of the target.</param>
      /// <returns><c>true</c> if [is type or assignable from type] [the specified target type]; otherwise, <c>false</c>.</returns>
      public static bool IsTypeOrAssignableFromType
      (
         this Type mainType,
         Type targetType
      )
      {
         return mainType == targetType || targetType.IsAssignableFrom(mainType);
      }

      /// <summary>
      ///    Minimums the of two doubles.
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
         return Math.Min(Convert.ToSingle(width), Convert.ToSingle(height));
      }

      /// <summary>
      ///    Positions the of decimal.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns>System.Int32.</returns>
      public static int PositionOfDecimal(this string str)
      {
         return str.IndexOf(DECIMAL, StringComparison.InvariantCultureIgnoreCase);
      }

      /// <summary>
      ///    Randoms the string.
      /// </summary>
      /// <param name="strings">The strings.</param>
      /// <param name="emptyOK"></param>
      /// <returns>System.String.</returns>
      public static string RandomString(this string[] strings, bool emptyOK = false)
      {
         if (strings.IsEmpty())
         {
            return "";
         }

         var validStrings = emptyOK ? strings : strings.Where(s => s.IsNotEmpty()).ToArray();

         var nextRandomString = validStrings[GLOBAL_RANDOM.Next(0, validStrings.Length - 1)];

         return nextRandomString;
      }

      /// <summary>
      ///    Reverses the dictionary.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <typeparam name="U"></typeparam>
      /// <param name="template">The template.</param>
      /// <returns>ConcurrentDictionary&lt;U, T&gt;.</returns>
      public static ConcurrentDictionary<U, T> ReverseDictionary<T, U>(ConcurrentDictionary<T, U> template)
      {
         // Declare the return dictionary backwards
         var retDict = new ConcurrentDictionary<U, T>();

         foreach (var keyVauePair in template)
         {
            // Check the value and not the key
            if (!retDict.ContainsKey(keyVauePair.Value))
            {
               // Add the values backwards
               // retDict.AddOrUpdate(keyVauePair.Value, keyVauePair.Key, (k, v) => v);
               retDict.AddOrUpdate(keyVauePair.Value, keyVauePair.Key);
            }
         }

         return retDict;
      }

      /// <summary>
      ///    Rounds to int.
      /// </summary>
      /// <param name="floatVal">The float value.</param>
      /// <returns>System.Int32.</returns>
      public static int RoundToInt(this double floatVal)
      {
         return (int)Math.Round(floatVal, 0);
      }

      /// <summary>
      ///    Converts to roundedint.
      /// </summary>
      /// <param name="d">The d.</param>
      /// <returns>System.Int32.</returns>
      public static int ToRoundedInt(this double d)
      {
         return (int)Math.Round(d, 0);
      }

      /// <summary>
      ///    Converts to roundedlong.
      /// </summary>
      /// <param name="d">The d.</param>
      /// <returns>System.Int64.</returns>
      public static long ToRoundedLong(this double d)
      {
         return (long)Math.Round(d, 0);
      }

      /// <summary>
      ///    Returns the value for a key, if that key exists in the dictionary.
      /// </summary>
      /// <param name="dict">The dictionary.</param>
      /// <param name="key">The key.</param>
      /// <returns>System.String.</returns>
      /// <remarks>This is *not* thread safe</remarks>
      public static string TryToGetKeyValue
      (
         this IDictionary<string, string> dict,
         string key
      )
      {
         if (dict != null && dict.ContainsKey(key))
         {
            return dict[key];
         }

         return string.Empty;
      }

      public static T[] AppendArray<T>(this T[] existingArray, T[] arrayToAppend)
      {
         var retList = existingArray.ToList();
         retList.AddRange(arrayToAppend);
         return retList.ToArray();
      }
      
      public static bool IsFalse(this bool? b)
      {
         return (b.HasValue && !b.Value);
      }
   }
}
