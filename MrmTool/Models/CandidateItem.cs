using MrmLib;
using MrmTool.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage.Streams;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.ValueType;
            set
            {
                Candidate.ValueType = value;
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
                PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
                PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
            }
        }

        public string StringValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.StringValue;
            set
            {
                Candidate.StringValue = value;
                PropertyChanged?.Invoke(this, new(nameof(StringValue)));
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
                PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
                PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
            }
        }

        public byte[] DataValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.DataValue;
            set
            {
                Candidate.DataValue = value;
                PropertyChanged?.Invoke(this, new(nameof(DataValue)));
                PropertyChanged?.Invoke(this, new(nameof(DataValueBuffer)));
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
                PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
                PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
            }
        }

        public IBuffer DataValueBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.DataValueBuffer;
            set
            {
                Candidate.DataValueBuffer = value;
                PropertyChanged?.Invoke(this, new(nameof(DataValue)));
                PropertyChanged?.Invoke(this, new(nameof(DataValueBuffer)));
                PropertyChanged?.Invoke(this, new(nameof(Type)));
                PropertyChanged?.Invoke(this, new(nameof(ValueType)));
                PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
                PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
            }
        }

        public IReadOnlyList<Qualifier> CandidateQualifiers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.Qualifiers;
            set
            {
                Candidate.Qualifiers = value;
                PropertyChanged?.Invoke(this, new(nameof(Qualifiers)));
                PropertyChanged?.Invoke(this, new(nameof(CandidateQualifiers)));
            }
        }

        public bool IsExportable
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.ValueType is not ResourceValueType.Path;
        }

        public bool IsPathCandidate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Candidate.ValueType is ResourceValueType.Path;
        }

        public void SetValue(string value)
        {
            Candidate.SetValue(value);
            PropertyChanged?.Invoke(this, new(nameof(StringValue)));
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
            PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
        }

        public void SetValue(byte[] value)
        {
            Candidate.SetValue(value);
            PropertyChanged?.Invoke(this, new(nameof(DataValue)));
            PropertyChanged?.Invoke(this, new(nameof(DataValueBuffer)));
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
            PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
        }

        public void SetValue(IBuffer value)
        {
            Candidate.SetValue(value);
            PropertyChanged?.Invoke(this, new(nameof(DataValue)));
            PropertyChanged?.Invoke(this, new(nameof(DataValueBuffer)));
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
            PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
        }

        public void SetValue(ResourceValueType valueType, string value)
        {
            Candidate.SetValue(valueType, value);
            PropertyChanged?.Invoke(this, new(nameof(Type)));
            PropertyChanged?.Invoke(this, new(nameof(ValueType)));
            PropertyChanged?.Invoke(this, new(nameof(IsExportable)));
            PropertyChanged?.Invoke(this, new(nameof(IsPathCandidate)));
        }

        public static implicit operator CandidateItem(ResourceCandidate candidate) => new(candidate);
        public static implicit operator ResourceCandidate(CandidateItem item) => item.Candidate;
    }
}
