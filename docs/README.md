
> ## NOW MAUI READY !!!

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

## SharedUtils Is Open Source; Enjoy Our Entire Suite

### *Shared Utils (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.SharedUtils)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.SharedUtils)

### *The Smart DI Container (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.SmartDI)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.SmartDI)

### *Responsive Tasks (MAUI Ready!)*

[GutHub](https://github.com/marcusts/Com.MarcusTS.ResponsiveTasks)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.ResponsiveTasks)

### *PlatformIndependentShared (MAUI Ready!)*

[GutHub](https://github.com/marcusts/PlatformIndependentShared)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.PlatformIndependentShared)

### *UI.XamForms*

[GutHub](https://github.com/marcusts/UI.XamForms)

[NuGet](https://www.nuget.org/packages/Com.MarcusTS.UI.XamForms)

### *The Modern App Demo*

[GutHub](https://github.com/marcusts/Com.MarcusTS.ModernAppDemo)

&nbsp;
![](https://gitlab.com/marcusts1/nugetimages/-/raw/master/Modern_App_Demo_Master_FINAL.gif)
