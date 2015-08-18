using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Общие сведения об этой сборке предоставляются следующим набором
// атрибутов. Отредактируйте значения этих атрибутов, чтобы изменить
// общие сведения об этой сборке.
[assembly: AssemblyTitle("BridgeNote")]
[assembly: AssemblyDescription("Program for protocoling bridge card game.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("BUBLiK")]
[assembly: AssemblyProduct("BridgeNote")]
[assembly: AssemblyCopyright("Copyright © BUBLiK 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Установка значения False в параметре ComVisible делает типы в этой сборке невидимыми
// для COM-компонентов. Если необходим доступ к типу в этой сборке из
// COM, следует установить атрибут ComVisible в TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("3f43db81-3daa-4d44-95c1-c0b6a599adf6")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии
//      Номер построения
//      Редакция
//
[assembly: AssemblyVersion("1.0.0.0")]

// Следующий атрибут служит для подавления предупреждения FxCop "CA2232: Microsoft.Usage: добавьте к сборке STAThreadAttribute",
// так как приложение для устройства не поддерживает поток STA.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2232:MarkWindowsFormsEntryPointsWithStaThread")]
