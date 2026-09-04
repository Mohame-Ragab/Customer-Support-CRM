using System.Text.Json;
using System.Text.Json.Nodes;

namespace CustomerSupportCRM.Application.Common.Auditing;

/// <summary>
/// Strips sensitive values from audit-log metadata before it is persisted
/// (security-admin/view-audit-logs). Matches property names case-insensitively
/// at any nesting depth and replaces their value with the literal string
/// <c>"[REDACTED]"</c>. No audit entry may ever contain a password, token, or
/// other secret.
/// </summary>
public static class SensitiveFieldRedactor
{
    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "pwd", "token", "accesstoken", "refreshtoken", "secret",
        "apikey", "authorization", "clientsecret", "passwordhash", "passwordsalt",
        "initialpassword", "newpassword", "currentpassword",
    };

    /// <summary>Redacts sensitive values and returns the result as a <see cref="JsonNode"/> (or <c>null</c> if <paramref name="metadata"/> is <c>null</c>).</summary>
    public static JsonNode? Redact(object? metadata)
    {
        if (metadata == null)
        {
            return null;
        }

        var json = JsonSerializer.Serialize(metadata);
        var node = JsonNode.Parse(json);
        RedactNode(node);
        return node;
    }

    private static void RedactNode(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var key in obj.Select(kv => kv.Key).ToList())
                {
                    if (SensitiveKeys.Contains(key))
                    {
                        obj[key] = "[REDACTED]";
                    }
                    else
                    {
                        RedactNode(obj[key]);
                    }
                }

                break;

            case JsonArray array:
                foreach (var item in array)
                {
                    RedactNode(item);
                }

                break;
        }
    }
}
