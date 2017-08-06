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
  /// Ascii modes.
  /// </summary>
  public enum AsciiModes
  {
    /// <summary>
    /// It affects the entire screen.
    /// </summary>
    Screen,

    /// <summary>
    /// It affects depending on the depth of the scene.
    /// </summary>
    Depth,

    /// <summary>
    /// Only has effect on objects of a layer.
    /// </summary>
    Layer,
  }
}
