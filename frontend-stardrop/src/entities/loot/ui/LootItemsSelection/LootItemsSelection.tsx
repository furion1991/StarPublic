import Image from 'next/image'
import cn from 'classnames'

import { LootItem } from '../LootItem/LootItem'

import classes from './LootItemsSelection.module.scss'

type Item = {
  inventoryId?: string
  itemId: string
  name: string
  rarity: number
  sellPrice: number
  image: string
}

type LootItemsSelectionProps = {
  items: Item[]
  selectedItem: Item | null
  onItemSelect: (item: Item) => void
}

export const LootItemsSelection = ({
  items,
  selectedItem,
  onItemSelect
}: LootItemsSelectionProps) => {
  return (
    <>
      {items.map(({ name, inventoryId, itemId, image, sellPrice, rarity }) => {
        return (
          <li
            key={inventoryId || itemId}
            className={classes.listItem}
            onClick={() => {
              onItemSelect({ name, inventoryId, itemId, image, sellPrice, rarity })
            }}
          >
            <LootItem
              className={classes.lootItem}
              name={name}
              image={image}
              price={sellPrice}
              rarity={rarity}
            />

            <div
              className={cn(classes.itemSelectedContainer, {
                [classes.selected]:
                  `${selectedItem?.itemId}-${selectedItem?.inventoryId}` ===
                  `${itemId}-${inventoryId}`
              })}
            >
              <Image src='/icons/checkmark-circle-white.svg' width={73} height={73} alt='Галочка' />
            </div>
          </li>
        )
      })}
    </>
  )
}
