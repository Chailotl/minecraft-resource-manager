# Minecraft Resource Manager
A Minecraft JSON resource manager and data-driven asset generator.

Usage should be very straight-forward. First you load your resource folder, and then you can click on the buttons and lists to open up their respective files in your preferred text editor. If a button is greyed out, that means that file does not exist.

The generate button opens up a 2nd window where you can create entirely new blocks or add missing files. Everything should be self-explanatory, and the text boxes have tooltips as well, but if you're still not sure what it all means here is a quick run-down:
- Mod: The name of your mod i.e. `minecraft`
- Block: The name of your block i.e. `redstone_wire`
- Item: The name of your block's item i.e. `redstone`
- Alternate item: Used by loot tables that may drop a different item i.e. coal from coal ore, cobblestone from stone
- Texture: The name of your texture

Several common template files have been included to make it trivial to add new blocks, fences, slabs, doors, etc. and more can be added by following the guide below.

Make sure to edit recipes and advancements afterwards as they will have default values! (The slab blockstate template may also require editing)

![](https://i.imgur.com/zEuhoFs.png)

## I want to make my own templates!
In the template folder are 6 additional folders. For most use cases, you will only need a singular file. Below are tokens the program will recognize and replace:
| Token | Name |
| ----- | ---- |
| `$MOD`  | Mod name |
| `$BLOCK` | Block name |
| `$ITEM` | Item name |
| `$ITEM2` | Alternate item name |
| `$TEX`  | Texture name |

For multi-file templates, create a subfolder with multiple files who's names begin with the subfolder's name. Let's use the fence template as an example, if the subfolder is called "fence", then all files inside this folder must begin with the word "fence".
```
> block models
 > fence
  | fence_inventory
  | fence_post
  | fence_side
```
