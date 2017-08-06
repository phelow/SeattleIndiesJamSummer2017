///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Ascii.
//
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace AsciiImageEffect
{
  /// <summary>
  /// Ascii text charsets.
  /// </summary>
  public enum AsciiCharsets
  {
    /// <summary>
    /// Lucida, 5x8, 94 chars.
    /// </summary>
    Lucida_5x8_94,

    /// <summary>
    /// Courier, 8x12, 94 chars.
    /// </summary>
    Courier_8x12_94,

    /// <summary>
    /// Times New Roman, 11x15, 4 chars.
    /// </summary>
    TimesNewRoman_11x15_4,

    /// <summary>
    /// Internal use only.
    /// You must set manually the texture and the number of characters with SetCustomCharset().
    /// </summary>
    Custom,
  }
}