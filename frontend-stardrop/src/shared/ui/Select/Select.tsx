'use client'

// import ReactSelect, { GroupBase, Props as ReactSelectProps } from 'react-select'
import { GroupBase, Props as ReactSelectProps } from 'react-select'
import { Controller, useFormContext } from 'react-hook-form'
import cn from 'classnames'
import { useState } from 'react'
import Image from 'next/image'

import dynamic from 'next/dynamic'
const ReactSelect = dynamic(() => import('react-select'), { ssr: false })

import classes from './Select.module.scss'

type SelectProps = {
  name: string
}

export const Select = <
  Option,
  IsMulti extends boolean = false,
  Group extends GroupBase<Option> = GroupBase<Option>
>({
  name,
  options,
  placeholder,
  noOptionsMessage,
  components
}: ReactSelectProps<Option, IsMulti, Group> & SelectProps) => {
  const [isMenuOpen, setMenuOpen] = useState(false)
  const { control } = useFormContext()

  return (
    <Controller
      control={control}
      name={name}
      render={({ field }) => (
        <ReactSelect
          {...field}
          placeholder={placeholder}
          options={options}
          noOptionsMessage={noOptionsMessage}
          menuIsOpen={isMenuOpen}
          onMenuOpen={() => {
            setMenuOpen(true)
          }}
          onMenuClose={() => {
            setMenuOpen(false)
          }}
          classNames={{
            container: () => classes.selectContainer,
            control: () => classes.select,
            valueContainer: () => classes.valueContainer,
            indicatorSeparator: () => classes.indicatorSeparator,
            indicatorsContainer: () => classes.indicatorsContainer,
            placeholder: ({ isFocused }) =>
              cn(classes.placeholder, {
                [classes.hidden]: isFocused
              }),
            input: () => classes.input,
            singleValue: () => classes.singleValue,
            menu: () => classes.menu,
            menuList: () => classes.menuList,
            option: () => classes.option
          }}
          // @ts-expect-error: ''
          components={{
            ...components,
            DropdownIndicator: () => {
              return (
                <div
                  className={cn(classes.arrow, {
                    [classes.rotated]: isMenuOpen
                  })}
                >
                  <Image src='/img/select-arrow.png' width={9} height={5} alt='Стрелка' />
                </div>
              )
            }
          }}
        />
      )}
    />
  )
}
