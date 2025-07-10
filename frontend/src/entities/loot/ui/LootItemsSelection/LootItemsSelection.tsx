import Image from 'next/image'
import cn from 'classnames'

import { LootItem } from '../LootItem/LootItem'

import classes from './LootItemsSelection.module.scss'

type Item = {
  id: string
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
      {items.map(({ name, id, image, sellPrice, rarity }) => {
        return (
          <li
            key={id}
            className={classes.listItem}
            onClick={() => {
              onItemSelect({ name, id, image, sellPrice, rarity })
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
                [classes.selected]: selectedItem?.id === id
              })}
            >
              <Image src='/icons/checkmark-white.svg' width={73} height={73} alt='Галочка' />
            </div>
          </li>
        )
      })}
    </>
  )
}
