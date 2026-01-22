using MrmLib;
using MrmTool.Common;
using System.ComponentModel;

namespace MrmTool.Models
{
    public partial class CandidateItem(ResourceCandidate candidate) : INotifyPropertyChanged
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
        public string Qualifiers => Candidate.Qualifiers.Count is 0 ? "(None)" : string.Join(", ",
            Candidate.Qualifiers.Select(q => $"({q.AttributeName} {q.Operator.Symbol} {q.Value}{(q.Priority is { } p && p != 0 ? $", Priority = {p}" : string.Empty)}{(q.FallbackScore is { } s && s != 0 ? $", Fallback Score = {s}" : string.Empty)})"));

        public event PropertyChangedEventHandler? PropertyChanged;

        public ResourceValueType ValueType
        {
            get => Candidate.ValueType;
            set
            {
                Candidate.ValueType = value;
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            }
        }

        public string StringValue
        {
            get => Candidate.StringValue;
            set
            {
                Candidate.StringValue = value;
                PropertyChanged?.Invoke(this, new(nameof(StringValue)));
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            }
        }

        public byte[] DataValue
        {
            get => Candidate.DataValue;
            set
            {
                Candidate.DataValue = value;
                PropertyChanged?.Invoke(this, new(nameof(DataValue)));
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            }
        }

        public IReadOnlyList<Qualifier> CandidateQualifiers
        {
            get => Candidate.Qualifiers;
            set
            {
                Candidate.Qualifiers = value;
                PropertyChanged?.Invoke(this, new(nameof(Qualifiers)));
                PropertyChanged?.Invoke(this, new(nameof(CandidateQualifiers)));
            }
        }

        public void SetValue(string value)
        {
            Candidate.SetValue(value);
            PropertyChanged?.Invoke(this, new(nameof(StringValue)));
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
        }

        public void SetValue(byte[] value)
        {
            Candidate.SetValue(value);
            PropertyChanged?.Invoke(this, new(nameof(DataValue)));
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
        }

        public void SetValue(ResourceValueType valueType, string value)
        {
            Candidate.SetValue(valueType, value);
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
        }

        public static implicit operator CandidateItem(ResourceCandidate candidate) => new(candidate);
        public static implicit operator ResourceCandidate(CandidateItem item) => item.Candidate;
    }
}
