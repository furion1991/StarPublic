import { LootItem, LootRarity } from '@/entities/loot'

import classes from './CaseLootList.module.scss'
import { PriceWithCurrency } from '@/shared/ui'

type Loot = {
  id: string
  name: string
  rarity: LootRarity
  isVisible: boolean
  game: string
  image: string
  sellPrice: number
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

  const getRarityLabel = (rarity: LootRarity) => {
    switch (rarity) {
      case LootRarity.LEGENDARY:
        return 'Легендарный'
      case LootRarity.MYTHICAL:
        return 'Мифический'
      case LootRarity.EPIC:
        return 'Эпический'
      case LootRarity.SUPER_RARE:
        return 'Сверхредкий'
      case LootRarity.RARE:
        return 'Редкий'
      case LootRarity.COMMON:
        return 'Обычный'
    }
  }

  return (
    <ul className={classes.caseLootList}>
      {sortByRarirty(loot.filter(({ isVisible }) => isVisible)).map(
        ({ id, name, rarity, image, sellPrice }) => {
          return (
            <li key={id}>
              <LootItem className={classes.lootItem} image={image} name={name} rarity={rarity} />

              <div className={classes.lootItemDetails}>
                <p>{getRarityLabel(rarity)}</p>

                <PriceWithCurrency>
                  {new Intl.NumberFormat('de-DE').format(sellPrice)}
                </PriceWithCurrency>
              </div>
            </li>
          )
        }
      )}
    </ul>
  )
}
