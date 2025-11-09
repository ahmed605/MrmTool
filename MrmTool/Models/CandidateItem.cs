using MrmLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrmTool.Models
{
    public class CandidateItem(ResourceCandidate candidate)
    {
        public ResourceCandidate Candidate { get; } = candidate;

        public string Type => Candidate.ValueType switch
        {
            ResourceValueType.String => "String",
            ResourceValueType.Path => "File Path",
            ResourceValueType.EmbeddedData => "Embedded Data",
            _ => "Unknown",
        };

        // TODO: Support custom operators and single-operand qualifiers (are these even used anywhere?)
        public string Qualifers => Candidate.Qualifiers.Count is 0 ? "(None)" : string.Join(", ",
            Candidate.Qualifiers.Select(q => $"({q.AttributeName} {q.Operator.Symbol} {q.Value}{(q.Priority is { } p && p != 0 ? $", Priority = {p}" : string.Empty)}{(q.FallbackScore is { } s && s != 0 ? $", Fallback Score = {s}" : string.Empty)})"));

        public static implicit operator CandidateItem(ResourceCandidate candidate) => new(candidate);
        public static implicit operator ResourceCandidate(CandidateItem item) => item.Candidate;
    }
}
