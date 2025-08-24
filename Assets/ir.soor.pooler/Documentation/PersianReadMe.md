# Pooler

![Unity](https://img.shields.io/badge/Unity-6000.0.30f1%2B-black?logo=unity)
![License](https://img.shields.io/github/license/AbolfazlHo/Pooler)
![GitHub last commit](https://img.shields.io/github/last-commit/AbolfazlHo/Pooler)
![Version](https://img.shields.io/badge/version-1.1.0-blue)

Pooler یک پکیج یونیتی است که برای ساده‌سازی و بهینه‌سازی Object Pooling در پروژه‌های شما طراحی شده است. این پکیج یک راه قدرتمند و قابل استفاده مجدد برای مدیریت اشیای pooled فراهم می‌کند و یک API تمیز و رویدادهای چرخه حیات قابل تنظیم را ارائه می‌دهد. این امر می‌تواند با کاهش سربار instantiating و destroying GameObjects، به بهبود قابل توجه عملکرد منجر شود.

##اجزای اصلی


این پکیج شامل سه اسکریپت اصلی است:

* `Poolable.cs`:
 
    این اسکریپت یک MonoBehaviour است که آن را به هر GameObject که می‌خواهید pool شود، اضافه می‌کنید. این اسکریپت رویدادهای UnityEvents را فراهم می‌کند که در مراحل مختلف چرخه حیات شیء، مانند زمان ایجاد، بازیابی از pool یا بازگرداندن به pool، فعال می‌شوند. این به شما امکان می‌دهد تا به راحتی منطق سفارشی را برای مقداردهی اولیه، فعال‌سازی و غیرفعال‌سازی متصل کنید، بدون نیاز به مدیریت دستی این حالت‌ها در کد خود.

* `Pooler.cs`: 

    یک کلاس `[Serializable]` که یک `Object Pool` را مدیریت می‌کند. این کلاس از کلاس داخلی `ObjectPool`<T> یونیتی استفاده می‌کند و شامل فیلدهای سریالایز شده برای نام `pool`، ظرفیت پیش‌فرض و حداکثر ظرفیت است. همچنین لیستی از` prefab`های `Poolable` را نگهداری می‌کند که به `pool` اجازه می‌دهد یک نوع را به صورت تکی یا تصادفی از بین چندین نوع، `instantiate` کند.

* `PoolsManager.cs`: 

    یک اسکریپت `MonoBehaviour` است که به عنوان یک مرکز اصلی برای مدیریت چندین نمونه `Pooler` عمل می‌کند. می‌توان آن را به یک `GameObject` در صحنه خود متصل کرد تا تمام `Object Pool`های شما را از یک مکان مدیریت کند. این اسکریپت متدهایی برای افزودن، ایجاد، بازیابی و حذف `pool`ها بر اساس نام‌های منحصربه‌فرد آن‌ها فراهم می‌کند و مدیریت نیازهای `pooling` پروژه شما را آسان می‌سازد.
    
## ویژگی‌ها


* `Pooling` انعطاف‌پذیر: 

    `Pooler` می‌تواند یک نوع `Prefab` را مدیریت کند یا از لیستی از `Prefab`های مختلف `instantiate` کند. شما می‌توانید انتخاب کنید که `instantiate` کردن آن‌ها به صورت تصادفی باشد یا به صورت ترتیبی (چرخه‌ای)، که این امر کنترل بیشتری بر رفتار `spawning` به شما می‌دهد.

* رویدادهای چرخه حیات سفارشی:
 
    کامپوننت `Poolable` رویدادهای `UnityEvents` را برای `OnCreate`، `OnGet` و `OnRelease` فراهم می‌کند و به شما کنترل دقیقی بر رفتار اشیاء `pooled` می‌دهد.

* مدیریت متمرکز:
 
    `PoolsManager` به شما امکان می‌دهد تمام `Object Pool`های خود را از یک مکان، چه از طریق `Inspector` و چه از طریق کد، مدیریت کنید.

* بهینگی:
 
    این پکیج با بهره‌گیری از `ObjectPool<T>` بومی یونیتی، یک راه‌حل بسیار بهینه‌سازی شده برای کاهش `Garbage Collection` و هزینه‌های `instantiation` ارائه می‌دهد.

## شروع به کار

### نصب

۱- Unity Package Manager را باز کنید.

۲- روی دکمه `+` در بالا سمت چپ کلیک کنید.

۳- `Add package from git URL...` را انتخاب کنید.

۴- آدرس زیر را وارد کنید:

 ```
 https://github.com/AbolfazlHo/Pooler.git?path=Assets/ir.soor.pooler.
```

۵- روی `Add` کلیک کنید.


### استفاده

#### ۱- ساخت یک  Poolable Prefab:

* یک `GameObject` که می‌خواهید `pool` کنید، ایجاد کنید.

* اسکریپت `Poolable.cs` را به آن اضافه کنید.

* در `Inspector`، هر `UnityEvents` لازم را تنظیم کنید 

* از این `GameObject` یک `Prefab` بسازید.


####۲- تنظیم PoolsManager:

* در صحنه خود یک `GameObject` خالی جدید ایجاد کنید.

* اسکریپت `PoolsManager`.cs را به آن اضافه کنید.

* در `Inspector`، یک `Pooler` جدید به لیست `All Poolers` اضافه کنید.

* به `Pooler` یک نام منحصربه‌فرد بدهید.

* Prefab Poolable خود را به لیست `Objects to Pool` بکشید.

* ظرفیت پیش‌فرض `Pool` و حداکثر ظرفیت `Pool` را در صورت نیاز تنظیم کنید.

* اگر چندین `Prefab` دارید، گزینه `Pool Randomly` را علامت بزنید تا `instantiation` تصادفی فعال شود.

#### ۳-  ایجاد Pool:

اگر می‌خواهید `pool` هنگام شروع صحنه آماده باشد، تیک گزینه `Generate All Pools on Awake` را در `Inspector` مربوط به `PoolsManager` بزنید.

همچنین، می‌توانید با فراخوانی `PoolsManager.GenerateObjectPool("YourPoolName")` از یک اسکریپت دیگر، آن را به صورت دستی ایجاد کنید.

#### ۴- استفاده از Pool:

برای دریافت یک شیء از `pool`:

```csharp
Pooler myPooler = poolsManager.GetPooler("YourPoolName");
Poolable pooledObject = myPooler.ObjectPool.Get();
```
برای بازگرداندن یک شیء به `pool`:

```csharp
myPooler.ObjectPool.Release(pooledObject);
```

برای افزودن یک poolable prefab جدید به یک `Pooler` که پیش‌تر ایجاد شده است:

```csharp
myPooler.AddPoolablePrefab(newPoolablePrefab);
```
    