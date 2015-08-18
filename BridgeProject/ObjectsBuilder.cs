using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BridgeProject
{
    static public class ClassBuilder
    {
        static public object CreateInstance(Type type)
        {
            // data:
            if (type == typeof(IntData))
                return new IntData();
            else if (type == typeof(BoolData))
                return new BoolData();
            else if (type == typeof(CardsDistribution))
                return new CardsDistribution();
            else if (type == typeof(ZoneSwitcher))
                return new ZoneSwitcher();
            else if (type == typeof(PairSwitcher))
                return new PairSwitcher();
            else if (type == typeof(QuarterSwitcher))
                return new QuarterSwitcher();
            else if (type == typeof(FitsSwitcher))
                return new FitsSwitcher();
            else if (type == typeof(OnersSwitcher))
                return new OnersSwitcher();
            else if (type == typeof(Contract))
                return new Contract();
            else if (type == typeof(Result))
                return new Result();
            else if (type == typeof(SimpleScore))
                return new SimpleScore();
            // controls:
            else if (type == typeof(DealInfoControl))
                return new DealInfoControl();
            else if (type == typeof(DealInfoControl_split))
                return new DealInfoControl_split();
            else if (type == typeof(ShowTextControl))
                return new ShowTextControl();
            else if (type == typeof(ShowTextControl_Center))
                return new ShowTextControl_Center();
            else if (type == typeof(SwitcherControl_Orange))
                return new SwitcherControl_Orange();
            else if (type == typeof(SwitcherControl_Orange_Center))
                return new SwitcherControl_Orange_Center();
            else if (type == typeof(ContractSelectControl))
                return new ContractSelectControl();
            else if (type == typeof(ResultSelectControl))
                return new ResultSelectControl();
            else if (type == typeof(ShowSimpleScore))
                return new ShowSimpleScore();
            else if (type == typeof(TextBoxInTable))
                return new TextBoxInTable();
            // unknown:
            else
            {
                throw new Exception("Конструктор объектов: неизвестный тип " + type.ToString());
                return type.Assembly.CreateInstance(type.AssemblyQualifiedName);
            }
        }
    }
}
