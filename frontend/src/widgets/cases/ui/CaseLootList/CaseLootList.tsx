import { LootItem, LootRarity } from '@/entities/loot'

import classes from './CaseLootList.module.scss'

type Loot = {
  id: string
  name: string
  rarity: LootRarity
  isVisible: boolean
  game: string
  image: string
}

type CaseLootListProps = {
  loot: Loot[]
}

export const CaseLootList = ({ loot }: CaseLootListProps) => {
  const sortByRarirty = (items: Loot[]) => {
    const rarityOrder = [
      LootRarity.LEGENDARY,
      LootRarity.MYTHICAL,
      LootRarity.EPIC,
      LootRarity.SUPER_RARE,
      LootRarity.RARE,
      LootRarity.COMMON
    ]

    return items.sort((a, b) => rarityOrder.indexOf(a.rarity) - rarityOrder.indexOf(b.rarity))
  }

  return (
    <ul className={classes.caseLootList}>
      {sortByRarirty(loot.filter(({ isVisible }) => isVisible)).map(
        ({ id, name, rarity, image }) => {
          return (
            <li key={id}>
              <LootItem className={classes.lootItem} image={image} name={name} rarity={rarity} />
            </li>
          )
        }
      )}
    </ul>
  )
}
