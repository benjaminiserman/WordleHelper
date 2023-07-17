namespace WordleHelper.Classes;
using WordleHelper.Enums;

internal static class GuessHelper
{
	public static Response[] ConvertToResponseArray(this string wordleResponse) => wordleResponse.Select(c => c switch
	{
		'g' or 'G' => Response.Green,
		'y' or 'Y' => Response.Yellow,
		_ => Response.White
	}).ToArray();
}
