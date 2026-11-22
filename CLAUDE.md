# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## General

Follow the shared general rules: [general.md](https://raw.githubusercontent.com/dzaba1/instructions/main/general.md)

The project rules below add to them and win where they conflict.

## C#

Follow the shared C# rules: [csharp.md](https://raw.githubusercontent.com/dzaba1/instructions/main/csharp.md) (it pulls in the general coding rules and, for the ASP.NET Core hosts, the ASP.NET Core rules).

The project rules below add to them and win where they conflict.

- **Nullable:** the existing projects are legacy and keep `<Nullable>disable</Nullable>`; don't enable it there. New projects must enable nullable reference types.
