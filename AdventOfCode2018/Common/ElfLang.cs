using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Common
{
    class ElfLang
    {
        public static Dictionary<string, Func<OpCode>> Mapping = new Dictionary<string, Func<OpCode>>
        {
            {"addr", () => new AddR() },
            {"addi", () => new AddI() },
            {"mulr", () => new MulR() },
            {"muli", () => new MulI() },
            {"banr", () => new BanR() },
            {"bani", () => new BanI() },
            {"borr", () => new BorR() },
            {"bori", () => new BorI() },
            {"setr", () => new SetR() },
            {"seti", () => new SetI() },
            {"gtir", () => new GtIR() },
            {"gtri", () => new GtRI() },
            {"gtrr", () => new GtRR() },
            {"eqir", () => new EqIR() },
            {"eqri", () => new EqRI() },
            {"eqrr", () => new EqRR() },
        };
    }

    interface OpCode
    {
        long[] Perform(long[] registers, int a, int b, int c);
    }

    class AddR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] + registers[b];

            return output;
        }
    }

    class AddI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] + b;

            return output;

        }
    }

    class MulR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] * registers[b];

            return output;
        }
    }

    class MulI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] * b;

            return output;
        }
    }

    class BanR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] & registers[b];

            return output;
        }
    }

    class BanI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] & b;

            return output;
        }
    }

    class BorR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] | registers[b];

            return output;
        }
    }

    class BorI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] | b;

            return output;
        }
    }

    class SetR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a];

            return output;
        }
    }

    class SetI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = a;

            return output;
        }
    }

    class GtIR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = a > registers[b] ? 1 : 0;

            return output;
        }
    }

    class GtRI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] > b ? 1 : 0;

            return output;
        }
    }

    class GtRR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] > registers[b] ? 1 : 0;

            return output;
        }
    }

    class EqIR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = a == registers[b] ? 1 : 0;

            return output;
        }
    }

    class EqRI : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] == b ? 1 : 0;

            return output;
        }
    }

    class EqRR : OpCode
    {
        public long[] Perform(long[] registers, int a, int b, int c)
        {
            var output = registers.ToArray();
            output[c] = registers[a] == registers[b] ? 1 : 0;

            return output;
        }
    }
}
