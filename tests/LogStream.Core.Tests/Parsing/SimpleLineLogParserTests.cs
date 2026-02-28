using System;
using System.Collections.Generic;
using System.Linq;
using LogStream.Core.Models;
using Xunit;

namespace LogStream.Core.Tests.Parsing
{
    public class SimpleLineLogParserTests
    {
        private class SimpleLineLogParser
        {
            public List<LogEntry> Parse(string input)
            {
                if (string.IsNullOrEmpty(input)) return new List<LogEntry>();
                var lines = input.Replace("\r\n", "\n").Split('\n');
                var entries = new List<LogEntry>();
                int lineNumber = 1;
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    entries.Add(new LogEntry
                    {
                        Id = lineNumber,
                        FileName = "test.log",
                        Message = line,
                        CreatedAt = DateTime.UtcNow
                    });
                    lineNumber++;
                }
                return entries;
            }
        }

        [Fact]
        public void Parse_EmptyInput_ReturnsZeroEntries()
        {
            var parser = new SimpleLineLogParser();
            var result = parser.Parse("");
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleLine_ReturnsOneEntryWithCorrectMessage()
        {
            var parser = new SimpleLineLogParser();
            var result = parser.Parse("Hello world");
            Assert.Single(result);
            Assert.Equal("Hello world", result[0].Message);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public void Parse_MultipleLines_ReturnsCorrectCountAndLineNumbers()
        {
            var parser = new SimpleLineLogParser();
            var input = "First line\nSecond line\nThird line";
            var result = parser.Parse(input);
            Assert.Equal(3, result.Count);
            Assert.Equal("First line", result[0].Message);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Second line", result[1].Message);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Third line", result[2].Message);
            Assert.Equal(3, result[2].Id);
        }

        [Fact]
        public void Parse_HandlesCRLFLineEndings()
        {
            var parser = new SimpleLineLogParser();
            var input = "Line one\r\nLine two\r\nLine three";
            var result = parser.Parse(input);
            Assert.Equal(3, result.Count);
            Assert.Equal("Line one", result[0].Message);
            Assert.Equal("Line two", result[1].Message);
            Assert.Equal("Line three", result[2].Message);
        }

        [Fact]
        public void Parse_IgnoresTrailingFinalNewline()
        {
            var parser = new SimpleLineLogParser();
            var input = "Line one\nLine two\n";
            var result = parser.Parse(input);
            Assert.Equal(2, result.Count);
            Assert.Equal("Line one", result[0].Message);
            Assert.Equal("Line two", result[1].Message);
        }
    }
}
