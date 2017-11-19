using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRx.Tests
{
    [TestClass]
    public class SelectWhereOptimizeTest
    {
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SelectSelect()
        {
            // Combine selector currently disabled.
            var selectselect = Observable.Range(1, 10)
                .Select(x => x)
                .Select(x => x * -1);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void WhereWhere()
        {
            var wherewhere = Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Where(x => x > 5);

            wherewhere.ToArrayWait().IsCollection(6, 8, 10);

            var wherewhere2 = Observable.Range(1, 10)
                .Where((x, i) => x % 2 == 0)
                .Where(x => x > 5);

            wherewhere2.ToArrayWait().IsCollection(6, 8, 10);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SelectWhere()
        {
            var selectWhere = Observable.Range(1, 10)
                .Select(x => x * x)
                .Where(x => x % 2 == 0);

            selectWhere.GetType().Name.Contains("SelectWhere").IsTrue();
            selectWhere.ToArrayWait().IsCollection(4, 16, 36, 64, 100);

            var selectWhere2 = Observable.Range(1, 10)
                .Select((x, i) => x * x)
                .Where(x => x % 2 == 0);

            selectWhere2.GetType().Name.Contains("SelectWhere").IsFalse();
            selectWhere2.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void WhereSelect()
        {
            var whereSelect = Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Select(x => x * x);

            whereSelect.GetType().Name.Contains("WhereSelect").IsTrue();
            whereSelect.ToArrayWait().IsCollection(4, 16, 36, 64, 100);

            var whereSelect2 = Observable.Range(1, 10)
                .Where((x, i) => x % 2 == 0)
                .Select(x => x * x);

            whereSelect2.GetType().Name.Contains("WhereSelect").IsFalse();
            whereSelect2.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
        }
    }
}
