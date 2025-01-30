using static IrrationalMath;

namespace MathTests
{
    [TestClass]
    public class IrrationalMathTest
    {
        [TestMethod]
        public void ToDecimalString()
        {
            BigFloat a = new([7, 3], 0, false);
            BigFloat b = new([15, 99, 72662], 0, false);
            BigFloat c = a.FlipBits();
            BigFloat d = new([7, 3, 0], 0, false);
            BigFloat e = new([108086391056891904], 4, false);

            Assert.AreEqual("27670116110564327431;0", a.ToString("dec"));
            Assert.AreEqual(d.ToString("dec"), a.ToString("dec"));
            Assert.AreEqual("6181399336302307658044844545133434068008975;0", b.ToString("dec"));
            Assert.AreEqual("-27670116110564327431;0", c.ToString("dec"));
            Assert.AreEqual("108086391056891904;4", e.ToString("dec"));
        }

        [TestMethod]
        public void ToString()
        {
            BigFloat a = new([7, 3], 8, false);
            BigFloat b = new([15, 99, 72662], 97, false);
            BigFloat c = a.FlipBits();
            BigFloat d = new([7, 3, 0], 8, false);

            Assert.AreEqual("108086391056891904.02734375", a.ToString());
            Assert.AreEqual(d.ToString(), a.ToString());
            Assert.AreEqual("39010114207744.0000000057625584304332733155243508086265214166494074278339343475607847722130827605724334716796875", b.ToString());
            Assert.AreEqual("-108086391056891904.02734375", c.ToString());
        }

        [TestMethod]
        //NEGATIVE NUMBERS NOT SUPPORTET!
        public void LeftShift()
        {
            BigFloat a = new([7, 3], 0, false);
            BigFloat b = new([15, 99, 72662], 0, false);

            Assert.AreEqual("55340232221128654862;0", (a << 1).ToString("dec"));
            Assert.AreEqual("12362798672604615316089689090266868136017950;0", (b << 1).ToString("dec"));

            Assert.AreEqual("221360928884514619448;0", (a << 3).ToString("dec"));
            Assert.AreEqual("49451194690418461264358756361067472544071800;0", (b << 3).ToString("dec"));

            Assert.AreEqual("127605887595351923831047279915904794624;0", (a << 62).ToString("dec"));
            Assert.AreEqual("28506672893541687353868969186110362683912262892660099278438400;0", (b << 62).ToString("dec"));

            Assert.AreEqual("255211775190703847662094559831809589248;0", (a << 63).ToString("dec"));
            Assert.AreEqual("57013345787083374707737938372220725367824525785320198556876800;0", (b << 63).ToString("dec"));

            Assert.AreEqual("510423550381407695324189119663619178496;0", (a << 64).ToString("dec"));
            Assert.AreEqual("114026691574166749415475876744441450735649051570640397113753600;0", (b << 64).ToString("dec"));

            Assert.AreEqual("1020847100762815390648378239327238356992;0", (a << 65).ToString("dec"));
            Assert.AreEqual("228053383148333498830951753488882901471298103141280794227507200;0", (b << 65).ToString("dec"));
        }

        [TestMethod]
        //NEGATIVE NUMBERS NOT SUPPORTET!
        public void RightShift()
        {
            BigFloat a = new([7, 3], 0, false);
            BigFloat b = new([15, 99, 72662], 0, false);

            Assert.AreEqual("13835058055282163715;0", (a >> 1).ToString("dec"));
            Assert.AreEqual("3090699668151153829022422272566717034004487;0", (b >> 1).ToString("dec"));

            Assert.AreEqual("3458764513820540928;0", (a >> 3).ToString("dec"));
            Assert.AreEqual("772674917037788457255605568141679258501121;0", (b >> 3).ToString("dec"));

            Assert.AreEqual("6;0", (a >> 62).ToString("dec"));
            Assert.AreEqual("1340377317883883439521990;0", (b >> 62).ToString("dec"));

            Assert.AreEqual("3;0", (a >> 63).ToString("dec"));
            Assert.AreEqual("670188658941941719760995;0", (b >> 63).ToString("dec"));

            Assert.AreEqual("1;0", (a >> 64).ToString("dec"));
            Assert.AreEqual("335094329470970859880497;0", (b >> 64).ToString("dec"));

            Assert.AreEqual("0;0", (a >> 65).ToString("dec"));
            Assert.AreEqual("167547164735485429940248;0", (b >> 65).ToString("dec"));

            Assert.AreEqual("0;0", (a >> 132).ToString("dec"));

        }

        [TestMethod]
        public void Addition()
        {
            BigFloat a = new([7, 3], 0, false);
            BigFloat b = new([15, 99, 72662], 0, false);
            BigFloat c = new BigFloat([611, 225], 0, false).FlipBits();

            Assert.AreEqual("6181399336302307658044872215249544632336406;0", (a + b).ToString("dec"));
            Assert.AreEqual((b + a).ToString("dec"), (a + b).ToString("dec"));

            Assert.AreEqual("6181399336302307658042769286425141743451564;0", (b + c).ToString("dec"));
            Assert.AreEqual("-6181399336302307658042769286425141743451564;0", (b.FlipBits() + c.FlipBits()).ToString("dec"));
            Assert.AreEqual((b + c).ToString("dec"), (c + b).ToString("dec"));

            Assert.AreEqual("-2047588592181760229980;0", (a + c).ToString("dec"));
            Assert.AreEqual("2047588592181760229980;0", (a.FlipBits() + c.FlipBits()).ToString("dec"));
        }

        [TestMethod]
        public void FlipBits()
        {
            BigFloat a = new BigFloat([7, 3], 0, false).FlipBits();
            Assert.AreEqual("-27670116110564327431;0", a.ToString("dec"));
            Assert.AreEqual("27670116110564327431;0", a.FlipBits().ToString("dec"));
        }
    }
}