
# SHARED UTILS: Making C# Quick, Easy and Safe

## Programs Should Not Be Redundant In Any Way

**SOLID** and **DRY** guidance declare that we should keep our C# code simple, narrow, and non-redundant.  For instance, simple code like this:

```csharp
var areSame = 
   string.Compare(mainStr, otherStr, CurrentCulture.CurrentCultureIgnoreCase) == 0;
```

... is both ungainly ***and*** unnecessary.  **DRY** does not refer to copying code like this, though it is certainly illegal:

```csharp
var areSameCheckedAgainRedundantly = 
   string.Compare(mainStr, otherStr, CurrentCulture.CurrentCultureIgnoreCase) == 0;
```

**DRY** means: no redundancy ***whatsoever***, regardless of where it lurks.  In this example, a parameter is redundant, and the check for 0 is both **static** *(not a variable but also not even a constant!)* **and** redundant.  The fix is easy -- just use our **SharedUtils** extensions to code this quickly and safely:

```csharp
using Com.MarcusTS.SharedUtils;

if (mainStr.isSameAs(otherStr))
{
}
```

If you are looking for *"not same as"*, then use this:

```csharp
using Com.MarcusTS.SharedUtils;

if (mainStr.isDifferentThan(otherStr))
{
}
```


**SharedUtils** provides similar comparison extensions for many C# types, including DateTime, numbers, and even IEnumerables!

**SharedUtils** also offers:

- A BetterObservableCollection that manages events more safely than in the C# version.
- A FlexibleStack that allows manipulation of a common stack.
- Reflection helpers
- Thread helpers,including a thread-safe Boolean
- Task helpers
- Numeric rounding

## SharedUtils Is Open Source; Enjoy Our Other Offerings

If you enjoy using **SharedUtils**, consider these other related projects:

| Git Hub                       &nbsp;|&nbsp; NuGet                    &nbsp;|&nbsp; Help API &nbsp;  |&nbsp;
| :---                                | :---                                 | :---                   |
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Shared Utils](https://github.com/marcusts/Com.MarcusTS.SharedUtils)  &nbsp;|&nbsp; [Shared Utils](https://www.nuget.org/packages/Com.MarcusTS.SharedUtils/) &nbsp;|&nbsp; [Shared Utils](https://github.com/marcusts/Com.MarcusTS.SharedUtils/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Smart DI Container](https://github.com/marcusts/Com.MarcusTS.SmartDI)  &nbsp;|&nbsp; [Smart DI Container](https://www.nuget.org/packages/Com.MarcusTS.SmartDI/) &nbsp;|&nbsp; [Smart DI Container](https://github.com/marcusts/Com.MarcusTS.SmartDI/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Shared Forms](https://github.com/marcusts/Com.MarcusTS.SharedForms)  &nbsp;|&nbsp; [Shared Forms](https://www.nuget.org/packages/Com.MarcusTS.SharedForms/) &nbsp;|&nbsp; [Shared Forms](https://github.com/marcusts/Com.MarcusTS.SharedForms/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Responsive Tasks](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks)  &nbsp;|&nbsp; [Responsive Tasks](https://www.nuget.org/packages/Com.MarcusTS.ResponsiveTasks/) &nbsp;|&nbsp; [Responsive Tasks](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Resp. Tasks Forms Support](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks.XamFormsSupport)  &nbsp;|&nbsp; [Resp. Tasks Forms Support](https://www.nuget.org/packages/Com.MarcusTS.ResponsiveTasks.XamFormsSupport/) &nbsp;|&nbsp; [Resp. Tasks Forms Support](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks.XamFormsSupport/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;
| [Modern App Demo](https://github.com/marcusts/Com.MarcusTS.ModernAppDemo)  &nbsp;|&nbsp;  &nbsp;|&nbsp; [Modern App Demo](https://github.com/marcusts/Com.MarcusTS.ModernAppDemo/docs/Help/index.html)	    &nbsp;|&nbsp;
|-------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;------------------------------&nbsp;|&nbsp;

\
&nbsp;
![](https://gitlab.com/marcusts1/nugetimages/-/raw/master/Modern_App_Demo_Master_FINAL.gif)