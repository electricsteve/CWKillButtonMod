# V1.1.0
2 Fixes
## Template patch
I had generated the mod with a template, and it included a template hook that made the price of your shopping cart increase a by a random amount, but I didn't remove it. So now I removed it.
## Not returning to surface
I don't know if this was actually a bug, but I think it was, so I fixed it. Basically if you killed yourself while underground you wouldn't be sent back to the surface.\
\
Detailed explanation:\
There is a method that checks if there are still players alive, and if not, it returns you to the surface. But this breaks if you die on the surface, so I hooked into it and cancelled the return to the surface if I just killed the player with the keybinding (Bc you respawn on the surface). Problem is, I didn't check if the player was on the surface when cancelling, so you would be soft locked underground if you killed yourself there.