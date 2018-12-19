using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day19
    {
        [Solution(19, 1)]
        public long Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var pointer = int.Parse(lines[0].Split(' ')[1]);
            var registers = new long[6];
            var instructions = lines.Skip(1).Select(it => new Instruction(it)).ToArray();

            while (registers[pointer] >= 0 && registers[pointer] < instructions.Length)
            {
                registers = instructions[registers[pointer]].Run(registers);
                registers[pointer]++;
            }

            return registers[0];
        }
        
        // Was not run to completion, worked out manually with notes included in project
        [Solution(19, 2)]
        public long Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var pointer = int.Parse(lines[0].Split(' ')[1]);
            var registers = new long[6] { 1, 0, 0, 0, 0, 0 };

            // Skip ahead
            //var registers = new long[6] { 0, 10551410, 10551403, 1, 0, 4 };
            var instructions = lines.Skip(1).Select(it => new Instruction(it)).ToArray();

            while (registers[pointer] >= 0 && registers[pointer] < instructions.Length)
            {
                registers = instructions[registers[pointer]].Run(registers);
                Console.WriteLine($"[{registers[0]}\t{registers[1]}\t{registers[2]}\t{registers[3]}\t{registers[4]}\t{registers[5]}\t]");
                registers[pointer]++;

            }

            return registers[0];
        }

        private class Instruction
        {
            private OpCode code;
            private int a;
            private int b;
            private int c;

            public Instruction(string input)
            {
                var items = input.Split(' ');
                code = ElfLang.Mapping[items[0]]();
                a = int.Parse(items[1]);
                b = int.Parse(items[2]);
                c = int.Parse(items[3]);
            }

            public long[] Run(long[] input)
            {
                return code.Perform(input, a, b, c);
            }
        }
    }
}
