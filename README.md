# Grimoire
Grimoire is a Unity Library that lets you create modules (using singletons) either for interfaces or back processes

##Usage for singleton

###Example

>using GLIB.Core;
>
>public class MyClass:Singleton<MyClass>{
>
>   public void Test(){}
>
>}

Access it in another class:

>public class OtherClass{
>
>   void AcessMyClass(){
>       MyClass.Instance.Test();
>   }
>
>}

That's it for now, tomorrow I will add more info.

