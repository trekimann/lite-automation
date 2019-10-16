using Magrathea.Executors;
using System.Xml.Linq;

namespace Magrathea.ifStatements
{
    public interface IIfStatements
    {
        bool resultOfCompare { get; }
        IMagratheaExecutor executor { get; set; }

        bool BooleanCompare(bool desiredCondition, bool actualValue);
        void If(XElement ifFunction);
        bool IntCompare(string condition, int valueFromMap, int intToCompareAgainst);
        bool NullCompare(bool condition, object value);
        bool SingleStringCompare(string condition, string value, string stringToMatch);
        bool StringCompare(string condition, string value, string stringToMatch);
    }
}