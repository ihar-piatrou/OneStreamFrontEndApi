# Code Analysis

Given the following:

```csharp
class Animal
{
    public virtual string speak(int x) { return "silence"; }
}

class Cat : Animal
{
    public string speak(int x) { return "meow"; }
}

class Dog : Animal
{
    public string speak(short x) { return "bow-wow"; }
}
```

### Question: 
Explain why the block below does not emit "bow-wow":

```csharp
Animal d = new Dog();
Console.Write(d.speak(0));
```

### Explanation:

`d.speak(0)` calls `Animal.speak(int)` because `Dog` does not override it, and `speak(short)` is not a match for `int`. Thus, it prints "silence".

### Fix:
Override `speak(int)` in `Dog`:

```csharp
class Dog : Animal
{
    public override string speak(int x) { return "bow-wow"; }
}
```

Or cast the argument to `short`:

```csharp
Console.Write(d.speak((short)0));  // Outputs: "bow-wow"
```
