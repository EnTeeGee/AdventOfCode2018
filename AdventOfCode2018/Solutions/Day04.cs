using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdventOfCode2018.Solutions
{
    class Day04
    {
        [Solution(4, 1)]
        public int Problem1(string input)
        {
            var grouped = GetGroupedHistories(input);
            var sleepiest = grouped.OrderByDescending(it => it.GetTotalDuration()).First();

            return sleepiest.GetMostProlificMinute() * sleepiest.Id;
        }

        [Solution(4, 2)]
        public int Problem2(string input)
        {
            var grouped = GetGroupedHistories(input);
            var sleepiest = grouped.Where(it => it.Sleeps.Any()).Select(it => new { it.Id, Info = it.GetMostProlificMinuteAndTimes() }).OrderByDescending(it => it.Info.Times).First();

            return sleepiest.Id * sleepiest.Info.Minute;
        }

        private List<GuardHistory> GetGroupedHistories(string input)
        {
            var items = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new GuardEvent(it)).OrderBy(it => it.DateTime);

            var histories = new List<GuardHistory>();
            var current = new List<GuardEvent>();
            foreach (var item in items)
            {
                if (item.EventType == EventType.StartShift)
                {
                    if (current.Count > 0)
                        histories.Add(new GuardHistory(current));

                    current = new List<GuardEvent> { item };
                }
                else
                    current.Add(item);
            }

            histories.Add(new GuardHistory(current));

            return histories.GroupBy(it => it.Id).Select(it => new GuardHistory(it.Key, it.SelectMany(gh => gh.Sleeps))).ToList();
        }

        private enum EventType
        {
            StartShift,
            FallAsleep,
            WakeUp
        }

        private class GuardEvent
        {
            public DateTime DateTime { get; }
            public int? GuardId { get; }
            public EventType EventType { get; }

            public GuardEvent(string input)
            {
                var timeInfo = input.Substring(1, 16);
                DateTime = DateTime.ParseExact(timeInfo, "yyyy-MM-dd' 'HH':'mm", CultureInfo.InvariantCulture);
                var items = input.Split(new[] { ' ', '#' }, StringSplitOptions.RemoveEmptyEntries);

                switch (items[2])
                {
                    case "Guard":
                        GuardId = int.Parse(items[3]);
                        EventType = EventType.StartShift;
                        break;
                    case "falls":
                        GuardId = null;
                        EventType = EventType.FallAsleep;
                        break;
                    case "wakes":
                        GuardId = null;
                        EventType = EventType.WakeUp;
                        break;
                }
            }
        }

        private class GuardHistory
        {
            public int Id { get; }
            public List<(int Start, int Duration)> Sleeps { get; }

            public GuardHistory(IEnumerable<GuardEvent> events)
            {
                Id = events.First().GuardId.GetValueOrDefault();
                Sleeps = new List<(int Start, int Duration)>();
                var remaining = events.Skip(1).ToArray();

                for(var i = 0; i < remaining.Length; i += 2)
                {
                    var startTime = remaining[i].DateTime.Minute;
                    var endTime = (i + 1 == remaining.Length) ? 60 : remaining[i + 1].DateTime.Minute;

                    Sleeps.Add((startTime, endTime - startTime));
                }
            }

            public GuardHistory(int id, IEnumerable<(int Start, int Duration)> sleeps)
            {
                Id = id;
                Sleeps = sleeps.ToList();
            }

            public int GetTotalDuration()
            {
                return Sleeps.Sum(it => it.Duration);
            }

            public int GetMostProlificMinute()
            {
                return Sleeps.Select(it => Enumerable.Range(it.Start, it.Duration))
                    .SelectMany(it => it)
                    .GroupBy(it => it)
                    .OrderByDescending(it => it.Count())
                    .First().Key;
            }

            public (int Minute, int Times) GetMostProlificMinuteAndTimes()
            {
                var peak = Sleeps.Select(it => Enumerable.Range(it.Start, it.Duration))
                    .SelectMany(it => it)
                    .GroupBy(it => it)
                    .OrderByDescending(it => it.Count())
                    .First();

                return (Minute: peak.Key, Times: peak.Count());
            }
        }
    }
}
