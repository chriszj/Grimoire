# Grimoire
Grimoire is a Unity Library that lets you create modules (using singletons) either for interfaces or back processes

##News

### Added Notification System utility, now you can display messages very easy, or notify progress when downloading a file.

### You can customize the window by setting the template path on the Class. Notification System is a subclass of UIModule.

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

