# ToolkitCore 2.0e (Experimental Fork)

**Status: Experimental** | **Use with caution in production environments**

A community-maintained fork of ToolkitCore with significant modifications and improvements. This version modernizes the codebase and addresses critical issues IS NOT compatible with existing Toolkit utilities.
You must use 2.0e Toolkit and 2.0e Utilities with this.

## ⚠️ Important Notes

- **This fork is EXPERIMENTAL** - Thorough testing recommended before use
- **May break with older Toolkit versions** - Check compatibility before upgrading
- **Examine all code changes** before considering integration

## 🛠 Major Modifications

### Critical Fixes
- **Threading Safety**: Replaced external threading with RimWorld's native thread management
- **Static API Preservation**: Maintained backward compatibility for Toolkit and Utilities

### Code Quality & Maintenance
- **TwitchLib Modernization**: Updated from v3.1.4 → 3.4.0 with complete event handler rewrites
- **Deprecation Cleanup**: Removed obsolete interfaces and methods
- **Translation Support**: Added and improved localization files
- **Code Modernization**: Updated to modern C# patterns and practices

## 📋 Change Impact

| Area | Impact Level | Notes |
|------|-------------|-------|
| User Management | High | Duplicate prevention + auto-repair |
| Threading | High | Now uses RimWorld native threads |
| Twitch Integration | Medium | API-compatible with major internal changes |
| Static APIs | Low | Backward compatible |
| Translations | Low | Additions and improvements |

## 📄 Licensing

- **Original Work**: ToolkitCore (MIT License)
- **Modifications**: © 2025 Captolamia - Licensed under GNU GPL v3
- **Community Preservation**: This fork maintains all original copyrights while adding substantial new work

## 🔧 Technical Details

**Development Investment**: 30+ hours of significant architectural work  
**Focus**: Stability, modernization, and community preservation  
**Philosophy**: Keep mods free and open source for community benefit

## 📞 Support

**This is an experimental fork** - use at your own risk. Please examine the code changes thoroughly before integration. Issues and pull requests are welcome for collaborative improvement.

---

*Preserving and modernizing abandoned code for the community since 2025*
