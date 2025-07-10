import Image from 'next/image'

import { TextField } from '@/shared/ui'

import classes from './ItemToUpgradeFilter.module.scss'

export const ItemToUpgradeFilter = () => {
  return (
    <>
      <>
        <TextField
          className={classes.itemsSearchField}
          name='priceSearch'
          autoComplete='off'
          placeholder='Цена от'
          endAdornment={<span className={classes.endAdornment}>₽</span>}
        />

        <TextField
          className={classes.itemsSearchField}
          name='nameSearch'
          autoComplete='off'
          placeholder='Поиск по названию'
          endAdornment={
            <span className={classes.endAdornment}>
              <Image src='/icons/loupe.svg' width={17} height={17} alt='Лупа' />
            </span>
          }
        />
      </>
    </>
  )
}
